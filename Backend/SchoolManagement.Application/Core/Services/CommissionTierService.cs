using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class CommissionTierService : ICommissionTierService
{
    private readonly ICommissionTierRepository _repository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;

    public CommissionTierService(
        ICommissionTierRepository repository,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
    }

    public async Task<CommissionTierResponseDto> CreateAsync(CommissionTierCommand command)
    {
        var tier = CommissionTierMapper.ToDomain(command);

        await _repository.AddAsync(tier);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "CommissionTier",
            entityId: tier.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(tier));

        return CommissionTierMapper.ToResponse(tier);
    }

    public async Task<CommissionTierResponseDto> UpdateAsync(Guid id, UpdateCommissionTierCommand command)
    {
        var tier = await _repository.GetByIdAsync(id);
        if (tier == null)
        {
            throw new NotFoundException($"CommissionTier with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(tier);

        tier.Update(
            minSalesCount: command.MinSalesCount,
            maxSalesCount: command.MaxSalesCount,
            amount: command.Amount,
            displayOrder: command.DisplayOrder);

        await _repository.UpdateAsync(tier);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "CommissionTier",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(tier));

        return CommissionTierMapper.ToResponse(tier);
    }

    public async Task DeleteAsync(Guid id)
    {
        var tier = await _repository.GetByIdAsync(id);
        if (tier == null)
        {
            throw new NotFoundException($"CommissionTier with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(tier);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "CommissionTier",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    public async Task<CommissionTierResponseDto> GetByIdAsync(Guid id)
    {
        var tier = await _repository.GetByIdAsync(id);
        if (tier == null)
        {
            throw new NotFoundException($"CommissionTier with ID {id} not found.");
        }

        return CommissionTierMapper.ToResponse(tier);
    }

    public async Task<List<CommissionTierResponseDto>> GetAllAsync()
    {
        var tiers = await _repository.GetAllAsync();
        return tiers.Select(CommissionTierMapper.ToResponse).ToList();
    }

    public async Task<List<CommissionTierResponseDto>> GetActiveAsync()
    {
        var tiers = await _repository.GetActiveAsync();
        return tiers.Select(CommissionTierMapper.ToResponse).ToList();
    }

    public async Task<CommissionTierResponseDto> ActivateAsync(Guid id)
    {
        var tier = await _repository.GetByIdAsync(id);
        if (tier == null)
        {
            throw new NotFoundException($"CommissionTier with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(tier);

        tier.Activate();
        await _repository.UpdateAsync(tier);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "CommissionTier",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(tier),
            message: "CommissionTier activated");

        return CommissionTierMapper.ToResponse(tier);
    }

    public async Task<CommissionTierResponseDto> DeactivateAsync(Guid id)
    {
        var tier = await _repository.GetByIdAsync(id);
        if (tier == null)
        {
            throw new NotFoundException($"CommissionTier with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(tier);

        tier.Deactivate();
        await _repository.UpdateAsync(tier);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "CommissionTier",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(tier),
            message: "CommissionTier deactivated");

        return CommissionTierMapper.ToResponse(tier);
    }

    private static object CreateAuditSnapshot(CommissionTier tier)
    {
        return new
        {
            tier.Id,
            tier.MinSalesCount,
            tier.MaxSalesCount,
            tier.Amount,
            tier.IsActive,
            tier.DisplayOrder
        };
    }
}
