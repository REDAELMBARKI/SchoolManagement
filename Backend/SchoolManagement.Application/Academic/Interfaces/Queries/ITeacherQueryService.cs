using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Interfaces.Queries;

public interface ITeacherQueryService : IEntityQuery<Teacher>
{
    Task<List<TeacherResponseDto>> GetAllResponsesAsync();
    Task<TeacherResponseDto?> GetResponseByIdAsync(Guid id);
    Task<Teacher?> GetByEmailAsync(string email);
    Task<Teacher?> GetBySlugAsync(string slug);
}
