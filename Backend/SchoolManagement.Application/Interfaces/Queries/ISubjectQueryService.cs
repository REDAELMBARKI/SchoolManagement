using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface ISubjectQueryService : IEntityQuery<Subject>
{
    Task<List<SubjectResponseDto>> GetAllResponsesAsync();
    Task<SubjectResponseDto?> GetResponseByIdAsync(Guid id);
}
