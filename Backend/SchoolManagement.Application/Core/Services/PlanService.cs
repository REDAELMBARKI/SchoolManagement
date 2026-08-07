using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _repository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    private readonly IPlanQueryService _query;
    public PlanService(
        IPlanRepository repository,
        IPlanQueryService query,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<PlanResponseDto>> GetAllAsync()
    {
        var plans = await _query.GetAllAsync();
        return plans.OrderBy(p => p.DurationMonths).Select(PlanMapper.ToResponse).ToList();
    }

    public async Task<List<PlanResponseDto>> GetActiveAsync()
    {
        var plans = await _repository.GetActiveAsync();
        return plans.Select(PlanMapper.ToResponse).ToList();
    }

    public async Task<PlanResponseDto> GetByIdAsync(Guid id)
    {
        var plan = await _query.GetByIdAsync(id);
        if (plan == null)
        {
            throw new NotFoundException($"Plan with ID {id} not found.");
        }
        return PlanMapper.ToResponse(plan);
    }

    public async Task<PlanResponseDto> CreateAsync(PlanCommand command)
    {
        var plan = PlanMapper.ToDomain(command, _currentUserContext.BranchId);
        var created = await _repository.AddAsync(plan);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Plan",
            entityId: created.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(created));

        return PlanMapper.ToResponse(created);
    }

    public async Task<PlanResponseDto> UpdateAsync(Guid id, UpdatePlanCommand command)
    {
        var plan = await _repository.GetByIdAsync(id);
        if (plan == null)
        {
            throw new NotFoundException($"Plan with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(plan);

        plan.UpdateName(command.Name);
        plan.UpdateDurationMonths(command.DurationMonths);
        plan.UpdateBaseAmount(command.BaseAmount);
        plan.UpdateDiscountPercent(command.DiscountPercent);
        plan.UpdateIsActive(command.IsActive);
        plan.UpdateRemainingAmountDueDate(command.RemainingAmountDueDays);

        var updated = await _repository.UpdateAsync(plan);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Plan",
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return PlanMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var plan = await _repository.GetByIdAsync(id);
        if (plan == null)
        {
            throw new NotFoundException($"Plan with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(plan);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "Plan",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    private static object CreateAuditSnapshot(Domain.Core.Entities.Plan plan)
    {
        return new
        {
            plan.Id,
            plan.Name,
            plan.DurationMonths,
            plan.BaseAmount,
            plan.DiscountPercent,
            plan.Amount,
            plan.IsActive,
            plan.RemainingAmountDueDays,
            plan.BranchId
        };
    }
}
