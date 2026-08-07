using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Interfaces.Queries;

public interface IGradeQueryService : IEntityQuery<Grade>
{
    Task<List<GradeResponseDto>> GetAllResponsesAsync();
    Task<GradeResponseDto?> GetResponseByIdAsync(Guid id);
    Task<List<Grade>> GetByStudentIdAsync(Guid studentId);
    Task<List<Grade>> GetByGroupTeacherIdAsync(Guid groupTeacherId);
}
