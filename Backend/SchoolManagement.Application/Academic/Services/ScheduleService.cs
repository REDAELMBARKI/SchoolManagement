using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;

namespace SchoolManagement.Application.Academic.Services;

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
