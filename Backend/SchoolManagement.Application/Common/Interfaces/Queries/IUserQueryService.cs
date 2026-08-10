using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Interfaces.Queries;

public interface IUserQueryService : IEntityQuery<DomainUser>
{
    Task<List<UserResponseDto>> GetAllResponsesAsync();
    Task<UserResponseDto?> GetResponseByIdAsync(Guid id);
    Task<DomainUser?> GetByEmailAsync(string email);
    Task<DomainUser?> GetBySlugAsync(string slug);
    Task<List<UserResponseDto>> GetByBranchIdAsync(Guid branchId);
    Task<List<UserResponseDto>> GetByRoleAsync(string role);
}
