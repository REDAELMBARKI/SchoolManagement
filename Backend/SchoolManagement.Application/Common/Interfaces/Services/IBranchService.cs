using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IBranchService
{
    Task<BranchResponseDto> CreateAsync(BranchCommand command);
    Task<BranchResponseDto> UpdateAsync(Guid id, UpdateBranchCommand command);
    Task DeleteAsync(Guid id);
    Task<BranchResponseDto> GetByIdAsync(Guid id);
    Task<List<BranchResponseDto>> GetAllAsync();
}
