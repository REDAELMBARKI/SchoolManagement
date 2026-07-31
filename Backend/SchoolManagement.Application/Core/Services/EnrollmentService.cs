using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;

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

        await EnsureNoDuplicateActiveEnrollmentAsync(command.StudentId, command.SubjectId);

        var availableGroups = await _groupQueryService.GetAvailableGroupsByLevelSubjectBranch(
            levelId: command.LevelId,
            subjectId: command.SubjectId,
            branchId: command.BranchId);

        command.GroupId = EvaluateStudentGroup(availableGroups, command.PreferedScheduleId, command.GroupId);
        var enrollment = EnrollmentMapper.ToDomain(command);
        var created = await _repository.AddAsync(enrollment);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Enrollment",
            entityId: created.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(created));

        return EnrollmentMapper.ToResponse(created);
    }

    public async Task<EnrollmentResponseDto> UpdateAsync(Guid id, UpdateEnrollmentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) throw new NotFoundException($"Enrollment with id {id} not found.");

        var oldValues = CreateAuditSnapshot(existing);

        existing.UpdateStudentId(command.StudentId);
        existing.UpdateSubjectId(command.SubjectId);
        existing.UpdateGroupId(command.GroupId);
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

        return EnrollmentMapper.ToResponse(updated);
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

    private Guid EvaluateStudentGroup(List<Group> availableGroups, Guid? PreferedScheduleId, Guid? groupId)
    {
        if (!availableGroups.Any())
            throw new UnAvailableResourceException("No available groups with free capacity for the selected level, subject, and branch.");

        if (groupId.HasValue && groupId.Value != Guid.Empty)
        {
            CheckGroupAvailability(availableGroups, groupId.Value);
            return groupId.Value;
        }

        return AssignNewGroup(availableGroups, PreferedScheduleId);
    }

    private Guid AssignNewGroup(List<Group> availableGroups, Guid? PreferedScheduleId)
    {
        var groupPrefered = availableGroups.FirstOrDefault(g => g.Schedule.Id == PreferedScheduleId);
        if (groupPrefered == null)
        {
            var first = availableGroups.FirstOrDefault();
            if (first == null)
                throw new UnAvailableResourceException("No available groups with free capacity for the selected level, subject, and branch.");
            return first.Id;
        }
        return groupPrefered.Id;
    }

    private void CheckGroupAvailability(List<Group> availableGroups, Guid groupId)
    {
        if (!availableGroups.Select(g => g.Id).Contains(groupId))
            throw new UnAvailableResourceException("The selected group is either full, belongs to a different subject/branch, or does not exist.");
    }

    private static object CreateAuditSnapshot(Enrollment enrollment)
    {
        return new
        {
            enrollment.Id,
            enrollment.EnrolledAt,
            enrollment.DroppedAt,
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
