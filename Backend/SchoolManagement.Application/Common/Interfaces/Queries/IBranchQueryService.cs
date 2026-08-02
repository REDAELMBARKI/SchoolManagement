using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Common.Interfaces.Queries;

public interface IBranchQueryService : IEntityQuery<Branch>
{
    Task<List<BranchResponseDto>> GetAllResponsesAsync();
    Task<BranchResponseDto?> GetResponseByIdAsync(Guid id);
}
