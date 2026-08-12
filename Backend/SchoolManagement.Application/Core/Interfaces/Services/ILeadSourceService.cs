using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface ILeadSourceService
{
    Task<List<LeadSourceResponseDto>> GetAllAsync();
    Task<LeadSourceResponseDto> GetByIdAsync(Guid id);
    Task<LeadSourceResponseDto> CreateAdLeadSourceAsync(AdLeadSourceCommand command);
    Task<LeadSourceResponseDto> CreateOpcLeadSourceAsync(OpcLeadSourceCommand command);
    Task DeleteAsync(Guid id);
}
