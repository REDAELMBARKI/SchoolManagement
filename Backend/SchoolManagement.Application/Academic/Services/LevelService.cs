using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Application.Academic.Services;

public class LevelService : ILevelService
{
    private readonly ILevelRepository _repository;
    private readonly ILevelQueryService _queryService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public LevelService(
        ILevelRepository repository,
        ILevelQueryService queryService,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _queryService = queryService;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<LevelResponseDto>> GetAllAsync()
    {
        // Use query service for non-tracking read operations
        return await _queryService.GetAllResponsesAsync();
    }

    public async Task<LevelResponseDto> GetByIdAsync(Guid id)
    {
        // Use query service for non-tracking read operations
        var level = await _queryService.GetResponseByIdAsync(id);
        if (level == null)
        {
            throw new NotFoundException($"Level with ID {id} not found.");
        }
        return level;
    }

    public async Task<LevelResponseDto> CreateAsync(LevelCommand command)
    {
        var level = LevelMapper.ToDomain(command, _currentUserContext.BranchId);
        
        // Use repository for tracking operations (Create/Update/Delete)
        var created = await _repository.AddAsync(level);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Level",
            entityId: created.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(created));

        return LevelMapper.ToResponse(created);
    }

    public async Task<LevelResponseDto> UpdateAsync(Guid id, UpdateLevelCommand command)
    {
        // Use repository for tracking operations - GetByIdAsync with tracking
        var level = await _repository.GetByIdAsync(id);
        if (level == null)
        {
            throw new NotFoundException($"Level with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(level);

        level.UpdateName(command.Name);
        level.UpdateOrder(command.Order);

        // Use repository for tracking operations
        var updated = await _repository.UpdateAsync(level);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Level",
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return LevelMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        // Use repository for tracking operations - GetByIdAsync with tracking
        var level = await _repository.GetByIdAsync(id);
        if (level == null)
        {
            throw new NotFoundException($"Level with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(level);

        // Use repository for tracking operations
        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "Level",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    private static object CreateAuditSnapshot(Domain.Academic.Entities.Level level)
    {
        return new
        {
            level.Id,
            level.Name,
            level.Order,
            level.BranchId
        };
    }
}
