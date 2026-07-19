using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Application.Interfaces.Services;

namespace SchoolManagement.Application.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleQueryService _queryService;

    public ScheduleService(IScheduleQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<GroupedScheduleDto> GetGroupScheduleAsync(int groupId)
    {
        throw new NotImplementedException();
    }
}
