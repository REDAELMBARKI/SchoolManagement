using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IGroupQueryService : IEntityQuery<Group>
{
    Task<List<GroupResponseDto>> GetAllResponsesAsync();
    Task<GroupResponseDto?> GetResponseByIdAsync(Guid id);
}
