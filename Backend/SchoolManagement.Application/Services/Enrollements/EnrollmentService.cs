using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Services.Enrollements;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repository;
    private readonly IEnrollmentQueryService _queryService;
    private readonly IGroupQueryService _groupQueryService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public EnrollmentService(
        IEnrollmentRepository repository,
        IEnrollmentQueryService queryService,
        IGroupQueryService groupQueryService,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _queryService = queryService;
        _groupQueryService = groupQueryService;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
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
            branchId: created.BranchId,
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
        existing.UpdatePlanId(command.PlanId);
        existing.UpdateNotes(command.Notes);

        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Enrollment",
            entityId: updated.Id,
            branchId: updated.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

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
                branchId: existing.BranchId,
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
            enrollment.Status,
            enrollment.Notes,
            enrollment.StudentId,
            enrollment.SubjectId,
            enrollment.GroupId,
            enrollment.BranchId,
            enrollment.PlanId
        };
    }
}
