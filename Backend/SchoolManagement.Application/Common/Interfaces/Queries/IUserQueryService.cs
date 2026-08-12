using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Interfaces.Queries;

public interface IUserQueryService : IEntityQuery<DomainUser>
{
    Task<List<DomainUserResponseDto>> GetAllResponsesAsync();
    Task<DomainUserResponseDto?> GetResponseByIdAsync(Guid id);
    Task<DomainUser?> GetByEmailAsync(string email);
    Task<DomainUser?> GetBySlugAsync(string slug);
    Task<List<DomainUserResponseDto>> GetByBranchIdAsync(Guid branchId);
    Task<List<DomainUserResponseDto>> GetByRoleAsync(string role);
}
