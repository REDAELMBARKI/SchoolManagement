using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Academic.Interfaces.Queries;

public interface IScheduleQueryService : IEntityQuery<Schedule>
{
    Task<List<ScheduleResponseDto>> GetAllResponsesAsync();
    Task<ScheduleResponseDto?> GetResponseByIdAsync(Guid id);
  
    Task<List<Schedule>> GetSchedulesByGroupIdAsync(Guid groupId);
    
    /// <summary>
    /// Gets all schedules for a specific room on a specific day.
    /// Used for room conflict detection.
    /// </summary>
    Task<List<Schedule>> GetRoomSchedulesAsync(Guid roomId, Guid dayId);
    
    /// <summary>
    /// Gets all schedules for a specific teacher on a specific day.
    /// Used for teacher conflict detection.
    /// </summary>
    Task<List<Schedule>> GetTeacherSchedulesAsync(Guid teacherId, Guid dayId);
}
