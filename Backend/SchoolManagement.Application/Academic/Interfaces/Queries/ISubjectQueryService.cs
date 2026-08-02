using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Academic.Interfaces.Queries;

public interface ISubjectQueryService : IEntityQuery<Subject>
{
    Task<List<SubjectResponseDto>> GetAllResponsesAsync();
    Task<SubjectResponseDto?> GetResponseByIdAsync(Guid id);
}
