using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Academic.Interfaces.Services;

public interface IScheduleService
{
    // CRUD Operations
    Task<bool> CreateSchedulesAsync(CreateSchedulesRequestDto request);
    Task<GroupScheduleResponseDto> GetGroupScheduleAsync(Guid groupId);
    Task<bool> UpdateScheduleAsync(Guid scheduleId, UpdateScheduleRequestDto request);
    Task<bool> DeleteScheduleAsync(Guid scheduleId);
    
    // AJAX Conflict Validation
    Task<RoomAvailabilityResponseDto> CheckRoomAvailabilityAsync(Guid roomId, Guid dayId, TimeOnly startTime, TimeOnly endTime, Guid? excludeScheduleId = null);
    Task<TeacherAvailabilityResponseDto> CheckTeacherAvailabilityAsync(Guid teacherId, Guid dayId, TimeOnly startTime, TimeOnly endTime, Guid? excludeScheduleId = null);
}