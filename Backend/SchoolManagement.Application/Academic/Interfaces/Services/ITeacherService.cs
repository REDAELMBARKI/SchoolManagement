using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;

namespace SchoolManagement.Application.Academic.Interfaces.Services;

public interface ITeacherService
{
    Task<TeacherResponseDto> CreateAsync(TeacherCommand command);
    Task<TeacherResponseDto> UpdateAsync(Guid id, UpdateTeacherCommand command);
    Task DeleteAsync(Guid id);
    Task<TeacherResponseDto> GetByIdAsync(Guid id);
    Task<List<TeacherResponseDto>> GetAllAsync();
}
