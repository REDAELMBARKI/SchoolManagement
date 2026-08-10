using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IDomainUserService
{
    // CRUD Operations
    Task<UserResponseDto> CreateAsync(DomainUserCommand command);
    Task<UserResponseDto> UpdateAsync(Guid id, UpdateDomainUserCommand command);
    Task DeleteAsync(Guid id);
    Task<UserResponseDto> GetByIdAsync(Guid id);
    Task<List<UserResponseDto>> GetAllAsync();

    // Branch Management
    Task<UserResponseDto> AssignBranchAsync(Guid userId, AssignBranchCommand command);
    Task<UserResponseDto> RemoveBranchAsync(Guid userId);

    // User Activation
    Task<UserResponseDto> ActivateAsync(Guid userId);
    Task<UserResponseDto> DeactivateAsync(Guid userId);

    // Query Operations
    Task<List<UserResponseDto>> GetByBranchIdAsync(Guid branchId);
    Task<List<UserResponseDto>> GetByRoleAsync(string role);
}
