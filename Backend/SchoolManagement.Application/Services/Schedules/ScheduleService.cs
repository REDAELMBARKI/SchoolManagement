using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Application.Interfaces.Services;

namespace SchoolManagement.Application.Services.Schedules;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleQueryService _queryService;

    public ScheduleService(IScheduleQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<GroupedScheduleDto> GetGroupScheduleAsync(Guid groupId)
    {
        throw new NotImplementedException();
    }
}
