using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repository;
    private readonly IEnrollmentQueryService _queryService;
    private readonly IGroupQueryService _groupQueryService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;
    private readonly IInvoiceQueryService _invoiceQueryService;
    private readonly IInvoiceService _invoiceService;
    private readonly ITransaction _transaction;

    public EnrollmentService(
        IEnrollmentRepository repository,
        IEnrollmentQueryService queryService,
        IGroupQueryService groupQueryService,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService,
        IInvoiceQueryService invoiceQueryService,
        IInvoiceService invoiceService,
        ITransaction transaction)
    {
        _repository = repository;
        _queryService = queryService;
        _groupQueryService = groupQueryService;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
        _invoiceQueryService = invoiceQueryService;
        _invoiceService = invoiceService;
        _transaction = transaction;
    }

    public async Task<List<EnrollmentResponseDto>> GetAllAsync()
    {
        return await _queryService.GetAllResponsesAsync();
    }

    public async Task<EnrollmentResponseDto?> GetByIdAsync(Guid id)
    {
        return await _queryService.GetResponseByIdAsync(id);
    }

    public async Task<EnrollmentResponseDto> CreateAsync(EnrollmentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        await _transaction.BeginTransactionAsync();
        try
        {
            await EnsureNoDuplicateActiveEnrollmentAsync(command.StudentId, command.SubjectId);

            var availableGroups = await _groupQueryService.GetAvailableGroupsByLevelSubjectBranch(
                levelId: command.LevelId,
                subjectId: command.SubjectId,
                branchId: command.BranchId);

            var selectedGroup = EvaluateStudentGroup(availableGroups, command.PreferedScheduleId, command.GroupId);
            if (!selectedGroup.HasAvailableSpace())
                throw new UnAvailableResourceException("The selected group has just reached capacity. Please refresh and try again.");

            // Touch the group row so EF can enforce optimistic concurrency on
            // the seat allocation save.
            selectedGroup.TouchCapacityGuard();

            command.GroupId = selectedGroup.Id;
            var enrollment = EnrollmentMapper.ToDomain(command);
            var created = await _repository.AddAsync(enrollment);

            await _auditLogService.StoreAsync(
                action: AuditLog.CreateAction(),
                entityName: "Enrollment",
                entityId: created.Id,
                branchId: _currentUserContext.BranchId,
                newValues: CreateAuditSnapshot(created));

            await _transaction.CommitTransactionAsync();

            return EnrollmentMapper.ToResponse(created);
        }
        catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
        {
            await _transaction.RollbackTransactionAsync();
            throw new ConcurrencyConflictException(
                "That group was updated by another request while we were saving this enrollment. Please refresh and try again.");
        }
        catch
        {
            await _transaction.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<EnrollmentResponseDto> UpdateAsync(Guid id, UpdateEnrollmentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        await _transaction.BeginTransactionAsync();
        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                throw new NotFoundException($"Enrollment with id {id} not found.");
            if (existing.BranchId != branchId)
                throw new DomainException("The enrollment does not belong to the current branch.");

            var oldValues = CreateAuditSnapshot(existing);
            var resolvedGroupId = existing.GroupId;
            var isGroupChanging = command.GroupId != Guid.Empty && command.GroupId != existing.GroupId;

            if (isGroupChanging)
            {
                var availableGroups = await _groupQueryService.GetAvailableGroupsByLevelSubjectBranch(
                    levelId: command.LevelId,
                    subjectId: command.SubjectId,
                    branchId: command.BranchId);

                var selectedGroup = EvaluateStudentGroup(availableGroups, command.PreferedScheduleId, command.GroupId);
                if (!selectedGroup.HasAvailableSpace())
                    throw new UnAvailableResourceException("The selected group has just reached capacity. Please refresh and try again.");

                // Touch the target group row so seat moves participate in the
                // same optimistic concurrency guard as enrollment creation.
                selectedGroup.TouchCapacityGuard();
                resolvedGroupId = selectedGroup.Id;
            }

            existing.UpdateStudentId(command.StudentId);
            existing.UpdateSubjectId(command.SubjectId);
            existing.UpdateGroupId(resolvedGroupId);
            existing.UpdateBranchId(command.BranchId);
            existing.UpdateNotes(command.Notes);

            if (command.PlanId.HasValue && command.PlanId.Value != Guid.Empty)
            {
                var latestPlan = existing.GetLatestPlan();
                if (latestPlan == null || latestPlan.Id != command.PlanId.Value)
                {
                    existing.AddPlan(command.PlanId.Value);
                }
            }

            var updated = await _repository.UpdateAsync(existing);

            await _auditLogService.StoreAsync(
                action: AuditLog.UpdateAction(),
                entityName: "Enrollment",
                entityId: updated.Id,
                branchId: _currentUserContext.BranchId,
                oldValues: oldValues,
                newValues: CreateAuditSnapshot(updated));

            await _transaction.CommitTransactionAsync();

            return EnrollmentMapper.ToResponse(updated);
        }
        catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
        {
            await _transaction.RollbackTransactionAsync();
            throw new ConcurrencyConflictException(
                "That group was updated by another request while we were saving this enrollment. Please refresh and try again.");
        }
        catch
        {
            await _transaction.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<EnrollmentResponseDto> AddCreditAsync(Guid id, decimal amount)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new NotFoundException($"Enrollment with id {id} not found.");

        var oldValues = CreateAuditSnapshot(existing);

        existing.AddCredit(amount);
        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Enrollment",
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return EnrollmentMapper.ToResponse(updated);
    }

    public async Task<EnrollmentResponseDto> DropEnrollmentAsync(DropEnrollmentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.DroppedByUserId = _currentUserContext.NameIdentifier;

        await _transaction.BeginTransactionAsync();
        try
        {
            var existing = await _repository.GetByIdAsync(command.EnrollmentId);
            if (existing is null)
                throw new NotFoundException($"Enrollment with id {command.EnrollmentId} not found.");
            if (existing.BranchId != branchId)
                throw new DomainException("The enrollment does not belong to the current branch.");

            var oldValues = CreateAuditSnapshot(existing);

            existing.DropEnrollment(command.Reason);
            existing.Group.ReleaseGroupCapacity();

            var cancelableInvoice = await _invoiceQueryService.GetLatestCancelableInvoiceByEnrollmentIdAsync(existing.Id);
            if (cancelableInvoice != null)
            {
                await _invoiceService.CancelInvoiceAsync(cancelableInvoice.Id, new CancelInvoiceCommand
                {
                    InvoiceId = cancelableInvoice.Id,
                    Reason = $"Enrollment dropped: {command.Reason}",
                    CancelledByUserId = command.DroppedByUserId
                });
            }

            var updated = await _repository.UpdateAsync(existing);

            await _auditLogService.StoreAsync(
                action: AuditLog.UpdateAction(),
                entityName: "Enrollment",
                entityId: updated.Id,
                branchId: branchId,
                oldValues: oldValues,
                newValues: CreateAuditSnapshot(updated),
                message: $"Dropped enrollment {updated.Id} for reason: {command.Reason}");

            await _transaction.CommitTransactionAsync();

            return EnrollmentMapper.ToResponse(updated);
        }
        catch
        {
            await _transaction.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<EnrollmentResponseDto> CompleteEnrollmentAsync(CompleteEnrollmentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.CompletedByUserId = _currentUserContext.NameIdentifier;

        var existing = await _repository.GetByIdAsync(command.EnrollmentId);
        if (existing is null)
            throw new NotFoundException($"Enrollment with id {command.EnrollmentId} not found.");
        if (existing.BranchId != branchId)
            throw new DomainException("The enrollment does not belong to the current branch.");

        var oldValues = CreateAuditSnapshot(existing);

        existing.CompleteEnrollment(command.Notes);

        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Enrollment",
            entityId: updated.Id,
            branchId: branchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated),
            message: $"Completed enrollment {updated.Id}");

        return EnrollmentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(id);

        if (existing != null)
        {
            await _auditLogService.StoreAsync(
                action: AuditLog.DeleteAction(),
                entityName: "Enrollment",
                entityId: existing.Id,
                branchId: _currentUserContext.BranchId,
                oldValues: CreateAuditSnapshot(existing));
        }
    }

    private async Task EnsureNoDuplicateActiveEnrollmentAsync(Guid studentId, Guid subjectId)
    {
        var duplicateExists = await _queryService.HasActiveEnrollmentForStudentSubjectAsync(studentId, subjectId);
        if (duplicateExists)
            throw new DomainException("Student already has an active enrollment for this subject.");
    }

    private Group EvaluateStudentGroup(List<Group> availableGroups, Guid? PreferedScheduleId, Guid? groupId)
    {
        if (!availableGroups.Any())
            throw new UnAvailableResourceException("No available groups with free capacity for the selected level, subject, and branch.");

        if (groupId.HasValue && groupId.Value != Guid.Empty)
        {
            return CheckGroupAvailability(availableGroups, groupId.Value);
        }

        return AssignNewGroup(availableGroups, PreferedScheduleId);
    }

    private Group AssignNewGroup(List<Group> availableGroups, Guid? PreferedScheduleId)
    {
        var groupPrefered = availableGroups.FirstOrDefault(g => g.Schedule.Id == PreferedScheduleId);
        if (groupPrefered == null)
        {
            var first = availableGroups.FirstOrDefault();
            if (first == null)
                throw new UnAvailableResourceException("No available groups with free capacity for the selected level, subject, and branch.");
            return first;
        }
        return groupPrefered;
    }

    private Group CheckGroupAvailability(List<Group> availableGroups, Guid groupId)
    {
        var group = availableGroups.FirstOrDefault(g => g.Id == groupId);
        if (group == null)
            throw new UnAvailableResourceException("The selected group is either full, belongs to a different subject/branch, or does not exist.");

        return group;
    }

    private static object CreateAuditSnapshot(Enrollment enrollment)
    {
        return new
        {
            enrollment.Id,
            enrollment.EnrolledAt,
            enrollment.DroppedAt,
            enrollment.CompletedAt,
            enrollment.Status,
            enrollment.Notes,
            enrollment.StudentId,
            enrollment.SubjectId,
            enrollment.GroupId,
            enrollment.BranchId,
            enrollment.CreditBalance
        };
    }
}
