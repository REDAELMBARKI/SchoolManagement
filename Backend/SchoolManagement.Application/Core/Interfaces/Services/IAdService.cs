using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IAdService
{
    Task<List<AdResponseDto>> GetAllAsync();
    Task<AdResponseDto?> GetByIdAsync(Guid id);
    Task<AdResponseDto> CreateAsync(AdCommand command);
    Task<AdResponseDto> UpdateAsync(Guid id, UpdateAdCommand command);
    Task DeleteAsync(Guid id);
}
