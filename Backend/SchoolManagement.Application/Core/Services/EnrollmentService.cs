using MediatR;
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
using SchoolManagement.Domain.Core.DomainEvents;
using SchoolManagement.Domain.Core.Enums;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repository;
    private readonly IEnrollmentQueryService _queryService;
    private readonly IGroupQueryService _groupQueryService;
    private readonly IScheduleQueryService _scheduleQueryService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;
    private readonly IInvoiceQueryService _invoiceQueryService;
    private readonly IInvoiceService _invoiceService;
    private readonly ITransaction _transaction;
    private readonly IMediator _mediator;

    public EnrollmentService(
        IEnrollmentRepository repository,
        IEnrollmentQueryService queryService,
        IGroupQueryService groupQueryService,
        IScheduleQueryService scheduleQueryService,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService,
        IInvoiceQueryService invoiceQueryService,
        IInvoiceService invoiceService,
        ITransaction transaction,
        IMediator mediator)
    {
        _repository = repository;
        _queryService = queryService;
        _groupQueryService = groupQueryService;
        _scheduleQueryService = scheduleQueryService;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
        _invoiceQueryService = invoiceQueryService;
        _invoiceService = invoiceService;
        _transaction = transaction;
        _mediator = mediator;
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

            // Publish domain events 
            foreach (var domainEvent in created.DomainEvents)
                await _mediator.Publish(domainEvent);
            created.ClearDomainEvents();

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

            // Publish domain events (e.g. commission clawback handler listens here)
            foreach (var domainEvent in updated.DomainEvents)
                await _mediator.Publish(domainEvent);
            updated.ClearDomainEvents();

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

    public async Task<EnrollmentResponseDto> TransferGroupAsync(TransferGroupCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        await _transaction.BeginTransactionAsync();
        try
        {
            // Load enrollment with current group
            var enrollment = await _repository.GetByIdAsync(command.EnrollmentId);
            if (enrollment is null)
                throw new NotFoundException($"Enrollment with id {command.EnrollmentId} not found.");
            if (enrollment.BranchId != branchId)
                throw new DomainException("The enrollment does not belong to the current branch.");

            var oldGroupId = enrollment.GroupId;

            // Load both groups
            var oldGroup = await _groupQueryService.GetByIdAsync(oldGroupId);
            var newGroup = await _groupQueryService.GetByIdAsync(command.NewGroupId);

            if (oldGroup is null)
                throw new NotFoundException($"Current group with id {oldGroupId} not found.");
            if (newGroup is null)
                throw new NotFoundException($"Target group with id {command.NewGroupId} not found.");
 
            // Validate same level and subject
            if (oldGroup.LevelId != newGroup.LevelId)
                throw new DomainException("Cannot transfer to a group with a different level.");
            if (oldGroup.SubjectId != newGroup.SubjectId)
                throw new DomainException("Cannot transfer to a group with a different subject.");

            // Check new group has space
            if (!newGroup.HasAvailableSpace())
                throw new UnAvailableResourceException("The target group has reached capacity.");

            // Schedule clash detection
            await ValidateNoScheduleConflictsAsync(enrollment.StudentId, command.EnrollmentId, command.NewGroupId);

            var oldValues = CreateAuditSnapshot(enrollment);

            // Transfer the group
            enrollment.TransferGroup(command.NewGroupId, command.Reason);

            // Touch groups for optimistic concurrency
            oldGroup.TouchCapacityGuard();
            newGroup.TouchCapacityGuard();

            var updated = await _repository.UpdateAsync(enrollment);

            // Publish domain events
            foreach (var domainEvent in updated.DomainEvents)
                await _mediator.Publish(domainEvent);
            updated.ClearDomainEvents();

            await _auditLogService.StoreAsync(
                action: AuditLog.UpdateAction(),
                entityName: "Enrollment",
                entityId: updated.Id,
                branchId: branchId,
                oldValues: oldValues,
                newValues: CreateAuditSnapshot(updated),
                message: $"Transferred from group {oldGroupId} to {command.NewGroupId}. Reason: {command.Reason}");

            await _transaction.CommitTransactionAsync();

            return EnrollmentMapper.ToResponse(updated);
        }
        catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
        {
            await _transaction.RollbackTransactionAsync();
            throw new ConcurrencyConflictException(
                "One of the groups was updated by another request. Please refresh and try again.");
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

    private async Task ValidateNoScheduleConflictsAsync(Guid studentId, Guid currentEnrollmentId, Guid newGroupId)
    {
        // Get all active enrollments for this student (excluding current one)
        var studentEnrollments = await _queryService.GetAllAsync();
        var activeEnrollments = studentEnrollments
            .Where(e => e.StudentId == studentId && 
                       e.Id != currentEnrollmentId && 
                       e.Status == EnrollmentStatus.Active)
            .ToList();

        // Get all schedule sessions for existing enrollments
        var existingSchedules = new List<Schedule>();
        foreach (var enrollment in activeEnrollments)
        {
            var schedules = await _scheduleQueryService.GetSchedulesByGroupIdAsync(enrollment.GroupId);
            existingSchedules.AddRange(schedules);
        }

        // Get all schedule sessions for new group
        var newGroupSchedules = await _scheduleQueryService.GetSchedulesByGroupIdAsync(newGroupId);

        // Check for conflicts
        foreach (var newSchedule in newGroupSchedules)
        {
            foreach (var existingSchedule in existingSchedules)
            {
                // Same day?
                if (newSchedule.DayId == existingSchedule.DayId)
                {
                    // Time overlap? (StartTime < existing.EndTime AND EndTime > existing.StartTime)
                    if (newSchedule.TimeSlot.StartTime < existingSchedule.TimeSlot.EndTime &&
                        newSchedule.TimeSlot.EndTime > existingSchedule.TimeSlot.StartTime)
                    {
                        throw new DomainException(
                            $"Schedule conflict detected: Student has another class on {existingSchedule.Day.Name} " +
                            $"from {existingSchedule.TimeSlot.StartTime:HH:mm} to {existingSchedule.TimeSlot.EndTime:HH:mm}.");
                    }
                }
            }
        }
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
