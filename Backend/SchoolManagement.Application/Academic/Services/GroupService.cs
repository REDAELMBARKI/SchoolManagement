using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Common.Utils;

namespace SchoolManagement.Application.Academic.Services;

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

        // Generate unique slug from Name + Period
        var baseSlug = $"{command.Name}-{command.Period}".ToLowerInvariant().Replace(" ", "-");
        command.Slug = await CustomSluger.Slug(
            async (slug) => await _repository.ExistsBySlugAsync(slug),
            baseSlug
        );

        Group entity = GroupMapper.ToDomain(command);
        var newEntity = await _repository.AddAsync(entity);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Group),
            entityId: newEntity.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(newEntity));

        return GroupMapper.ToResponse(newEntity);
    }

    public async Task<GroupResponseDto> GetByIdAsync(Guid id)
    {
        var group = await _query.GetResponseByIdAsync(id);
        if (group == null)
        {
            throw new NotFoundException($"Group with ID {id} not found.");
        }
        return group;
    }

    public async Task<List<GroupResponseDto>> GetAllAsync()
    {
        return await _query.GetAllResponsesAsync();
    }
    
    public async Task<GroupResponseDto> UpdateAsync(Guid id, UpdateGroupCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) throw new NotFoundException($"Group with id {id} not found");

        var oldValues = CreateAuditSnapshot(existing);

        // Generate unique slug if name or period changed
        if (existing.Name != command.Name || existing.Period != command.Period)
        {
            var baseSlug = $"{command.Name}-{command.Period}".ToLowerInvariant().Replace(" ", "-");
            command.Slug = await CustomSluger.Slug(
                async (slug) => await _repository.ExistsBySlugAsync(slug),
                baseSlug
            );
            existing.UpdateSlug(command.Slug);
        }

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
            branchId: _currentUserContext.BranchId,
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
                branchId: _currentUserContext.BranchId,
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
            group.Slug,
            group.Capacity,
            group.Period,
            group.LevelId,
            group.SubjectId,
            group.BranchId
        };
    }
}
