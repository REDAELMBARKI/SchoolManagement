using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface ICommercialAgentQueryService : IEntityQuery<CommercialAgent>
{
    Task<List<CommercialAgentResponseDto>> GetAllResponsesAsync();
    Task<CommercialAgentResponseDto?> GetResponseByIdAsync(Guid id);
}
