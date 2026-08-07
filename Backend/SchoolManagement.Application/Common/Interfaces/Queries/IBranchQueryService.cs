using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Interfaces.Queries;

public interface IBranchQueryService : IEntityQuery<Branch>
{
    Task<List<BranchResponseDto>> GetAllResponsesAsync();
    Task<BranchResponseDto?> GetResponseByIdAsync(Guid id);
    Task<Branch?> GetByNameAsync(string name);
    Task<Branch?> GetBySlugAsync(string slug);
    Task<List<Branch>> GetByCityAsync(string city);
}
