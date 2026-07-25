using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IPlanQueryService : IEntityQuery<Plan>
{
    Task<List<PlanResponseDto>> GetAllResponsesAsync();
    Task<PlanResponseDto?> GetResponseByIdAsync(Guid id);
}
