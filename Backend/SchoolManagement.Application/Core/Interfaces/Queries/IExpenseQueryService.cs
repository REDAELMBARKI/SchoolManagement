using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface IExpenseQueryService : IEntityQuery<Expense>
{
    /// <summary>Filtered query by branch, date range, category, and/or staff.</summary>
    Task<List<Expense>> GetFilteredAsync(
        Guid? branchId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ExpenseType? category = null,
        Guid? staffId = null);
}
