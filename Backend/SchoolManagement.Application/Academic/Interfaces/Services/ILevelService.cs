using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;

namespace SchoolManagement.Application.Academic.Interfaces.Services;

public interface ILevelService
{
    Task<List<LevelResponseDto>> GetAllAsync();
    Task<LevelResponseDto> GetByIdAsync(Guid id);
    Task<LevelResponseDto> CreateAsync(LevelCommand command);
    Task<LevelResponseDto> UpdateAsync(Guid id, UpdateLevelCommand command);
    Task DeleteAsync(Guid id);
}
