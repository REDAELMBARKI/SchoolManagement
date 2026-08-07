using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface ICommercialAgentService
{
    Task<CommercialAgentResponseDto> CreateAsync(CommercialAgentCommand command);
    Task<CommercialAgentResponseDto> UpdateAsync(Guid id, UpdateCommercialAgentCommand command);
    Task DeleteAsync(Guid id);
    Task<CommercialAgentResponseDto> GetByIdAsync(Guid id);
    Task<List<CommercialAgentResponseDto>> GetAllAsync();
}
