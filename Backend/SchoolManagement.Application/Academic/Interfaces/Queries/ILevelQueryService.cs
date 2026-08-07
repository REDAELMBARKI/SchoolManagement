using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Interfaces.Queries;

public interface ILevelQueryService : IEntityQuery<Level>
{
    Task<List<LevelResponseDto>> GetAllResponsesAsync();
    Task<LevelResponseDto?> GetResponseByIdAsync(Guid id);
    Task<Level?> GetByNameAsync(string name);
}
