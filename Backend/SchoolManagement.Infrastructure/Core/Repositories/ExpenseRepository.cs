using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class ExpenseRepository : Repository<Expense>, IExpenseRepository
{
    public ExpenseRepository(AppDbContext context) : base(context) { }

    public async Task<List<Expense>> GetByBranchAsync(Guid branchId)
    {
        return await Query()
            .Where(e => e.BranchId == branchId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }
}
