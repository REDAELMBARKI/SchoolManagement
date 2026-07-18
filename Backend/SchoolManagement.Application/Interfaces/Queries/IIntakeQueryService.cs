using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IIntakeQueryService : IEntityQuery<Intake>, ISluged
{
    Task<List<IntakeResponseDto>> GetAllResponsesAsync();
    Task<IntakeResponseDto?> GetResponseByIdAsync(Guid id);
}
