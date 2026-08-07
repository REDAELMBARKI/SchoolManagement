using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IPlatformService
{
    Task<PlatformResponseDto> CreateAsync(PlatformCommand command);
    Task<PlatformResponseDto> UpdateAsync(Guid id, UpdatePlatformCommand command);
    Task DeleteAsync(Guid id);
    Task<PlatformResponseDto> GetByIdAsync(Guid id);
    Task<List<PlatformResponseDto>> GetAllAsync();
}
