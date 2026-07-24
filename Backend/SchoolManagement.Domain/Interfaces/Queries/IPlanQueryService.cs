using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IPlanQueryService
{
    Task<List<Plan>> GetAllAsync();
    Task<Plan?> GetByIdAsync(Guid id);
    Task<bool> IsExistsAsync(Guid id);
    Task<List<PlanResponseDto>> GetAllResponsesAsync();
    Task<PlanResponseDto?> GetResponseByIdAsync(Guid id);
}
