using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repository;
    private readonly IExpenseQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public ExpenseService(
        IExpenseRepository repository,
        IExpenseQueryService query,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<ExpenseResponseDto>> GetAllAsync()
    {
        var expenses = await _query.GetAllAsync();
        return expenses.Select(ExpenseMapper.ToResponse).ToList();
    }

    public async Task<ExpenseResponseDto?> GetByIdAsync(Guid id)
    {
        var expense = await _query.GetByIdAsync(id);
        if (expense == null) return null;
        return ExpenseMapper.ToResponse(expense);
    }

    public async Task<List<ExpenseResponseDto>> GetFilteredAsync(
        Guid? branchId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ExpenseType? category = null,
        Guid? staffId = null)
    {
        var expenses = await _query.GetFilteredAsync(branchId, startDate, endDate, category, staffId);
        return expenses.Select(ExpenseMapper.ToResponse).ToList();
    }

    public async Task<ExpenseResponseDto> CreateAsync(ExpenseCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.BranchId = branchId;
        command.ProcessedByStaffId = _currentUserContext.NameIdentifier;

        var expense = ExpenseMapper.ToDomain(command);
        var created = await _repository.AddAsync(expense);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Expense),
            entityId: created.Id,
            branchId: branchId,
            newValues: CreateAuditSnapshot(created),
            message: $"Expense of {created.Amount:C} recorded for '{created.PayeeName}' ({created.Category}).");

        return ExpenseMapper.ToResponse(created);
    }

    public async Task<ExpenseResponseDto> UpdateAsync(Guid id, UpdateExpenseCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        var existing = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No expense found with id {id}");

        if (existing.BranchId != branchId)
            throw new DomainException("Expense does not belong to the current branch.");

        var oldValues = CreateAuditSnapshot(existing);

        existing.UpdateCategory(command.Category);
        existing.UpdatePayeeName(command.PayeeName);
        existing.UpdateDescription(command.Description);
        existing.UpdateAmount(command.Amount);
        existing.UpdateExpenseDate(command.ExpenseDate);
        existing.UpdatePaymentMethod(command.PaymentMethod);
        existing.UpdateReference(command.Reference);

        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Expense),
            entityId: updated.Id,
            branchId: branchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return ExpenseMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var branchId = _currentUserContext.BranchId;
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return;

        if (existing.BranchId != branchId)
            throw new DomainException("Expense does not belong to the current branch.");

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: nameof(Expense),
            entityId: existing.Id,
            branchId: branchId,
            oldValues: CreateAuditSnapshot(existing));
    }

    private static object CreateAuditSnapshot(Expense e) => new
    {
        e.Id,
        e.Category,
        e.PayeeName,
        e.Description,
        e.Amount,
        e.ExpenseDate,
        e.PaymentMethod,
        e.Reference,
        e.ProcessedByStaffId,
        e.BranchId
    };
}
