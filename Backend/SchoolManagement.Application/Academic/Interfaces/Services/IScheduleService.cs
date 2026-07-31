using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Academic.Interfaces.Services;

public interface IScheduleService
{
    Task<GroupedScheduleDto> GetGroupScheduleAsync(Guid groupId);
}