using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IExpenseService
{
    Task<List<ExpenseResponseDto>> GetAllAsync();
    Task<ExpenseResponseDto> GetByIdAsync(Guid id);
    Task<List<ExpenseResponseDto>> GetFilteredAsync(
        Guid? branchId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ExpenseType? category = null,
        Guid? staffId = null);
    Task<ExpenseResponseDto> CreateAsync(ExpenseCommand command);
    Task<ExpenseResponseDto> UpdateAsync(Guid id, UpdateExpenseCommand command);
    Task DeleteAsync(Guid id);
}
