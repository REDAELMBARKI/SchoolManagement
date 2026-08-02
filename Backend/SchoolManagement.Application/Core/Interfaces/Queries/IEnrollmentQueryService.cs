using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface IEnrollmentQueryService : IEntityQuery<Enrollment>
{
    Task<List<EnrollmentResponseDto>> GetAllResponsesAsync();
    Task<EnrollmentResponseDto?> GetResponseByIdAsync(Guid id);
    Task<bool> HasActiveEnrollmentForStudentSubjectAsync(Guid studentId, Guid subjectId);
}

