using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Repositories;

namespace SchoolManagement.Backend.Services;

public class ScheduleService
{
    private readonly ScheduleRepository _repository;

    public ScheduleService(ScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<GroupedScheduleDto> GetGroupScheduleAsync(int groupId)
    {
        return await _repository.GetGroupSchedule(groupId);
    }
}
