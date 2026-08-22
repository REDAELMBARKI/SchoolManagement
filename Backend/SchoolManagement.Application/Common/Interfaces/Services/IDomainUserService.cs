using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IDomainUserService
{
    // CRUD Operations
    Task<DomainUserResponseDto> CreateAsync(DomainUserCommand command);
    Task<DomainUserResponseDto> UpdateAsync(Guid id, UpdateDomainUserCommand command);
    Task DeleteAsync(Guid id);
    Task<DomainUserResponseDto> GetByIdAsync(Guid id);
    Task<List<DomainUserResponseDto>> GetAllAsync();

    // Branch Management
    Task<DomainUserResponseDto> AssignBranchAsync(Guid userId, AssignBranchCommand command);
    Task<DomainUserResponseDto> RemoveBranchAsync(Guid userId);

    // User Activation
    Task<DomainUserResponseDto> ActivateAsync(Guid userId);
    Task<DomainUserResponseDto> DeactivateAsync(Guid userId);

    // Role Conversion
    Task<DomainUserResponseDto> ConvertToStaffAsync(ConvertToStaffCommand command);

    // Query Operations
    Task<List<DomainUserResponseDto>> GetByBranchIdAsync(Guid branchId);
    Task<List<DomainUserResponseDto>> GetByRoleAsync(string role);
    Task<DomainUserResponseDto> GetByApplicationUserIdAsync(string applicationUserId);
}
