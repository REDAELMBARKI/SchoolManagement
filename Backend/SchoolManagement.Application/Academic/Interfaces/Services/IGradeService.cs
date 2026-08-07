using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;

namespace SchoolManagement.Application.Academic.Interfaces.Services;

public interface IGradeService
{
    Task<GradeResponseDto> CreateAsync(GradeCommand command);
    Task<GradeResponseDto> UpdateAsync(Guid id, UpdateGradeCommand command);
    Task DeleteAsync(Guid id);
    Task<GradeResponseDto> GetByIdAsync(Guid id);
    Task<List<GradeResponseDto>> GetAllAsync();
    Task<List<GradeResponseDto>> GetByStudentAsync(Guid studentId);
    Task<List<GradeResponseDto>> GetByGroupTeacherAsync(Guid groupTeacherId);
}
