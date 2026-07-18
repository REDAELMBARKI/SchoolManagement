using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IGenderQueryService : IEntityQuery<Gender>
{
    Task<List<GenderResponseDto>> GetAllResponsesAsync();
    Task<GenderResponseDto?> GetResponseByIdAsync(Guid id);
}
