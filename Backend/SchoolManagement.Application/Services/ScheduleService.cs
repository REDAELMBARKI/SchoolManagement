using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Application.Services;

public class ScheduleService
{
    private readonly IScheduleRepository _repository;

    public ScheduleService(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<GroupedScheduleDto> GetGroupScheduleAsync(int groupId)
    {
        return await _repository.GetGroupSchedule(groupId);
    }
}

