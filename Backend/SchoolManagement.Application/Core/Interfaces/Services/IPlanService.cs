using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IPlanService
{
    Task<List<PlanResponseDto>> GetAllAsync();
    Task<List<PlanResponseDto>> GetActiveAsync();
    Task<PlanResponseDto> GetByIdAsync(Guid id);
    Task<PlanResponseDto> CreateAsync(PlanCommand command);
    Task<PlanResponseDto> UpdateAsync(Guid id, UpdatePlanCommand command);
    Task DeleteAsync(Guid id);
}
