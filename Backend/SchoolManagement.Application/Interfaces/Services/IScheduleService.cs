using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IScheduleService
{
    Task<GroupedScheduleDto> GetGroupScheduleAsync(Guid groupId);
}