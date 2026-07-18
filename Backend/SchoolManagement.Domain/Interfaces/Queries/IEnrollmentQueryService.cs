using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IEnrollmentQueryService : IEntityQuery<Enrollment>
{
    Task<List<EnrollmentResponseDto>> GetAllResponsesAsync();
    Task<EnrollmentResponseDto?> GetResponseByIdAsync(Guid id);
}

