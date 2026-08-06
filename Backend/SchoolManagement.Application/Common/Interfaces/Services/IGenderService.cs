using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IGenderService
{
    Task<List<GenderResponseDto>> GetAllAsync();
    Task<GenderResponseDto?> GetByIdAsync(Guid id);
    Task<GenderResponseDto> CreateAsync(GenderCommand command);
    Task<GenderResponseDto> UpdateAsync(Guid id, UpdateGenderCommand command);
    Task DeleteAsync(Guid id);
}
