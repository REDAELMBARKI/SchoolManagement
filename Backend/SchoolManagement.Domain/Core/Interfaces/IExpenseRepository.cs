using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IExpenseRepository : IRepository<Expense>
{
    /// <summary>Returns all expenses for a given branch.</summary>
    Task<List<Expense>> GetByBranchAsync(Guid branchId);
}
