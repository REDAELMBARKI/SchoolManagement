using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IBranchQueryService : IEntityQuery<Branch>
{
    Task<List<BranchResponseDto>> GetAllResponsesAsync();
    Task<BranchResponseDto?> GetResponseByIdAsync(Guid id);
}
