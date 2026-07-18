using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Domain.Interfaces.Services;

public interface IScheduleService
{
    Task<GroupedScheduleDto> GetGroupScheduleAsync(int groupId);
}