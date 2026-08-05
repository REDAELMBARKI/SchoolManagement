using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Enums;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class ExpenseQueryService : IExpenseQueryService
{
    private readonly AppDbContext _context;

    public ExpenseQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        return await _context.Expenses
            .Include(e => e.Branch)
            .Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        return await _context.Expenses
            .Include(e => e.Branch)
            .Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Expenses
            .Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null)
            .AnyAsync(e => e.Id == id);
    }

    public async Task<List<Expense>> GetFilteredAsync(
        Guid? branchId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ExpenseType? category = null,
        Guid? staffId = null)
    {
        var query = _context.Expenses
            .Include(e => e.Branch)
            .Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null);

        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        if (startDate.HasValue)
            query = query.Where(e => e.ExpenseDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.ExpenseDate <= endDate.Value);

        if (category.HasValue)
            query = query.Where(e => e.Category == category.Value);

        if (staffId.HasValue)
            query = query.Where(e => e.ProcessedByStaffId == staffId.Value);

        return await query
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }
}
