using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IOpcService
{
    Task<List<OpcResponseDto>> GetAllAsync();
    Task<OpcResponseDto> GetByIdAsync(Guid id);
    Task<OpcResponseDto> CreateAsync(OpcCommand command);
    Task<OpcResponseDto> UpdateAsync(Guid id, UpdateOpcCommand command);
    Task DeleteAsync(Guid id);
}
