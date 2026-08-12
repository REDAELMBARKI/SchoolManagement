using Microsoft.Extensions.Options;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Application.Options;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class CommissionService : ICommissionService
{
    private readonly ICommissionRepository _repository;
    private readonly ICommissionTierRepository _tierRepository;
    private readonly IIntakeRepository _intakeRepository;
    private readonly ICommissionQueryService _query;
    private readonly ICommercialAgentQueryService _agentQueryService;
    private readonly CommissionSettings _settings;
    private readonly IAuditLogService _auditLogService;

    public CommissionService(
        ICommissionRepository repository,
        ICommissionTierRepository tierRepository,
        IIntakeRepository intakeRepository,
        ICommissionQueryService query,
        ICommercialAgentQueryService agentQueryService,
        IOptions<CommissionSettings> settings,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _tierRepository = tierRepository;
        _intakeRepository = intakeRepository;
        _query = query;
        _agentQueryService = agentQueryService;
        _settings = settings.Value;
        _auditLogService = auditLogService;
    }

    // ────────────────────────────────────────────────────────────
    // OPC — per enrollment event
    // ────────────────────────────────────────────────────────────

    public async Task ProcessOpcCommissionAsync(Guid enrollmentId, Guid studentId)
    {
        // Idempotency guard — never create two commissions for the same enrollment
        if (await _repository.OpcCommissionExistsForEnrollmentAsync(enrollmentId))
            return;

        // Walk: Student → Intake → LeadSource (OpcLeadSource) → OpcId
        var intake = await _intakeRepository.GetIntakeByStudentId(studentId);
        if (intake?.LeadSource is not OpcLeadSource opcLeadSource)
            return; // not OPC-sourced, nothing to do

        var opcId = opcLeadSource.OpcId;
        if (opcId == Guid.Empty)
            return;

        var now = DateTime.UtcNow;
        var periodMonth = new DateOnly(now.Year, now.Month, 1);

        // Don't create a commission if we're already past the salary lockout
        if (_settings.IsLocked(periodMonth, now))
            return;

        var commission = Commission.CreateForOpc(
            opcId: opcId,
            amount: _settings.OpcFlatAmount,
            periodMonth: periodMonth,
            enrollmentId: enrollmentId,
            branchId: intake.BranchId);

        await _repository.AddAsync(commission);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Commission),
            entityId: commission.Id,
            branchId: intake.BranchId,
            newValues: CreateAuditSnapshot(commission),
            message: $"OPC commission created (Approved) for enrollment {enrollmentId}");
    }

    // ────────────────────────────────────────────────────────────
    // Commercial Agent — monthly tiered
    // ────────────────────────────────────────────────────────────

    public async Task ProcessAgentMonthlyCommissionsAsync(int year, int month)
    {
        var periodMonth = new DateOnly(year, month, 1);
        var agents = await _agentQueryService.GetAllAsync();

        foreach (var agent in agents)
        {
            // Skip if already calculated for this period
            var existing = await _query.GetAgentCommissionForPeriodAsync(agent.Id, periodMonth);
            if (existing != null)
                continue;

            var salesCount = await _repository.CountAgentEnrollmentsForMonthAsync(agent.Id, year, month);
            if (salesCount == 0)
                continue;

            // Load tier from database
            var tier = await _tierRepository.FindTierForSalesCountAsync(salesCount);
            if (tier == null)
                continue;

            var commission = Commission.CreateForAgent(
                agentId: agent.Id,
                amount: tier.Amount,
                periodMonth: periodMonth,
                salesCount: salesCount,
                commissionTierId: tier.Id,
                branchId: agent.BranchId);

            await _repository.AddAsync(commission);

            await _auditLogService.StoreAsync(
                action: AuditLog.CreateAction(),
                entityName: nameof(Commission),
                entityId: commission.Id,
                branchId: agent.BranchId,
                newValues: CreateAuditSnapshot(commission),
                message: $"Agent commission created for agent {agent.Id}, period {periodMonth:yyyy-MM}, {salesCount} sales, tier {tier.MinSalesCount}-{tier.MaxSalesCount?.ToString() ?? "∞"}");
        }
    }

    // ────────────────────────────────────────────────────────────
    // Salary day lockout — runs on day 13 at 8pm UTC
    // Flips all Approved → Paid for the current month only
    // ────────────────────────────────────────────────────────────

    public async Task ProcessSalaryLockoutAsync(int year, int month)
    {
        var periodMonth = new DateOnly(year, month, 1);

        var approved = await _repository.GetApprovedByPeriodAsync(periodMonth);
        if (!approved.Any())
            return;

        foreach (var commission in approved)
        {
            var old = CreateAuditSnapshot(commission);
            commission.MarkAsPaid();
            await _repository.UpdateAsync(commission);

            await _auditLogService.StoreAsync(
                action: AuditLog.UpdateAction(),
                entityName: nameof(Commission),
                entityId: commission.Id,
                branchId: Guid.Empty,
                oldValues: old,
                newValues: CreateAuditSnapshot(commission),
                message: $"Commission locked and marked as Paid on salary day {year}-{month:D2}");
        }
    }

    // ────────────────────────────────────────────────────────────
    // Block — called when enrollment is dropped or manually by manager
    // ────────────────────────────────────────────────────────────

    public async Task<CommissionResponseDto> BlockCommissionAsync(Guid id, string reason)
    {
        var commission = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Commission {id} not found.");

        // Enforce salary lockout — once Paid nothing changes
        if (_settings.IsLocked(commission.PeriodMonth, DateTime.UtcNow))
            throw new DomainException("Cannot block commission — salary cutoff has already passed for this period.");

        var old = CreateAuditSnapshot(commission);
        commission.Block(reason);
        await _repository.UpdateAsync(commission);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Commission),
            entityId: id,
            branchId: Guid.Empty,
            oldValues: old,
            newValues: CreateAuditSnapshot(commission),
            message: $"Commission blocked. Reason: {reason}");

        return CommissionMapper.ToResponse(commission);
    }

    // ────────────────────────────────────────────────────────────
    // Block by enrollment — called by event handler on enrollment drop
    // ────────────────────────────────────────────────────────────

    public async Task BlockOpcCommissionByEnrollmentAsync(Guid enrollmentId, string reason)
    {
        var commission = await _repository.GetOpcCommissionByEnrollmentAsync(enrollmentId);
        if (commission == null)
            return; // no OPC commission for this enrollment, nothing to do

        // Already blocked or paid — skip silently
        if (commission.Status == CommissionStatus.Blocked || commission.Status == CommissionStatus.Paid)
            return;

        // Don't block if salary already went out
        if (_settings.IsLocked(commission.PeriodMonth, DateTime.UtcNow))
            return;

        var old = CreateAuditSnapshot(commission);
        commission.Block(reason);
        await _repository.UpdateAsync(commission);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Commission),
            entityId: commission.Id,
            branchId: Guid.Empty,
            oldValues: old,
            newValues: CreateAuditSnapshot(commission),
            message: $"OPC commission auto-blocked — enrollment {enrollmentId} was dropped. Reason: {reason}");
    }

    // ────────────────────────────────────────────────────────────
    // Queries
    // ────────────────────────────────────────────────────────────

    public async Task<List<CommissionResponseDto>> GetByEarnerAsync(Guid earnerId, EarnerType earnerType)
    {
        var commissions = await _query.GetByEarnerAsync(earnerId, earnerType);
        return commissions.Select(CommissionMapper.ToResponse).ToList();
    }

    public async Task<List<CommissionResponseDto>> GetByPeriodAsync(int year, int month)
    {
        var period = new DateOnly(year, month, 1);
        var commissions = await _query.GetByPeriodAsync(period);
        return commissions.Select(CommissionMapper.ToResponse).ToList();
    }

    public async Task<CommissionResponseDto> GetByIdAsync(Guid id)
    {
        var commission = await _query.GetByIdAsync(id)
            ?? throw new NotFoundException($"Commission {id} not found.");
        return CommissionMapper.ToResponse(commission);
    }

    public async Task<List<CommissionResponseDto>> GetAllAsync()
    {
        var commissions = await _query.GetAllAsync();
        return commissions.Select(CommissionMapper.ToResponse).ToList();
    }

    // ────────────────────────────────────────────────────────────
    // Lifecycle
    // ────────────────────────────────────────────────────────────

    public async Task<CommissionResponseDto> ApproveAsync(Guid id)
    {
        var commission = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Commission {id} not found.");

        var old = CreateAuditSnapshot(commission);
        commission.Approve();
        await _repository.UpdateAsync(commission);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Commission),
            entityId: id,
            branchId: Guid.Empty,
            oldValues: old,
            newValues: CreateAuditSnapshot(commission));

        return CommissionMapper.ToResponse(commission);
    }

    public async Task<CommissionResponseDto> MarkAsPaidAsync(Guid id)
    {
        var commission = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Commission {id} not found.");

        var old = CreateAuditSnapshot(commission);
        commission.MarkAsPaid();
        await _repository.UpdateAsync(commission);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Commission),
            entityId: id,
            branchId: Guid.Empty,
            oldValues: old,
            newValues: CreateAuditSnapshot(commission));

        return CommissionMapper.ToResponse(commission);
    }

    private static object CreateAuditSnapshot(Commission commission)
    {
        return new
        {
            commission.Id,
            commission.EarnerId,
            commission.EarnerType,
            commission.Amount,
            commission.PeriodMonth,
            commission.Status,
            commission.CommissionTierId,
            commission.SourceEnrollmentId,
            commission.SalesCountAtCalculation
        };
    }
}
