using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface IPlanQueryService : IEntityQuery<Plan>
{
    Task<List<PlanResponseDto>> GetAllResponsesAsync();
    Task<PlanResponseDto?> GetResponseByIdAsync(Guid id);
}
