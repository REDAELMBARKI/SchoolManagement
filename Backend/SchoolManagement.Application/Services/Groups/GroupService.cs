using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Services.Groups;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _repository;
    private readonly IGroupQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public GroupService(
        IGroupRepository repository,
        IGroupQueryService query,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<GroupResponseDto> CreateAsync(GroupCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        Group entity = GroupMapper.ToDomain(command);
        var newEntity = await _repository.AddAsync(entity);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Group),
            entityId: newEntity.Id,
            branchId: newEntity.BranchId,
            newValues: CreateAuditSnapshot(newEntity));

        return GroupMapper.ToResponse(newEntity);
    }

    public async Task<GroupResponseDto?> GetByIdAsync(Guid id)
    {
        return await _query.GetResponseByIdAsync(id);
    }

    public async Task<List<GroupResponseDto>> GetAllAsync()
    {
        return await _query.GetAllResponsesAsync();
    }
    
    public async Task<GroupResponseDto?> UpdateAsync(Guid id, UpdateGroupCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) throw new NotFoundException($"Group with id {id} not found");

        var oldValues = CreateAuditSnapshot(existing);

        existing.UpdateName(command.Name);
        existing.UpdateCapacity(command.Capacity);
        existing.UpdatePeriod(command.Period);
        existing.UpdateLevelId(command.LevelId);
        existing.UpdateSubjectId(command.SubjectId);
        existing.UpdateBranchId(command.BranchId);

        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Group),
            entityId: updated.Id,
            branchId: updated.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return GroupMapper.ToResponse(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(id);

        if (existing != null)
        {
            await _auditLogService.StoreAsync(
                action: AuditLog.DeleteAction(),
                entityName: nameof(Group),
                entityId: existing.Id,
                branchId: existing.BranchId,
                oldValues: CreateAuditSnapshot(existing));
        }

        return true;
    }

    private static object CreateAuditSnapshot(Group group)
    {
        return new
        {
            group.Id,
            group.Name,
            group.Capacity,
            group.Period,
            group.LevelId,
            group.SubjectId,
            group.BranchId,
            group.ScheduleId
        };
    }
}
