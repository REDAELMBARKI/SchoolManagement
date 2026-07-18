using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IScheduleQueryService : IEntityQuery<Schedule>
{
    Task<List<ScheduleResponseDto>> GetAllResponsesAsync();
    Task<ScheduleResponseDto?> GetResponseByIdAsync(Guid id);
}
