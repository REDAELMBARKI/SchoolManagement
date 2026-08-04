using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Application.Academic.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleQueryService _scheduleQueryService;
    private readonly ITimeSlotQueryService _timeSlotQueryService;
    private readonly IDayQueryService _dayQueryService;
    private readonly IRoomQueryService _roomQueryService;
    private readonly ITeacherQueryService _teacherQueryService;
    private readonly ISubjectQueryService _subjectQueryService;
    private readonly AppDbContext _context;

    public ScheduleService(
        IScheduleQueryService scheduleQueryService,
        ITimeSlotQueryService timeSlotQueryService,
        IDayQueryService dayQueryService,
        IRoomQueryService roomQueryService,
        ITeacherQueryService teacherQueryService,
        ISubjectQueryService subjectQueryService,
        AppDbContext context)
    {
        _scheduleQueryService = scheduleQueryService;
        _timeSlotQueryService = timeSlotQueryService;
        _dayQueryService = dayQueryService;
        _roomQueryService = roomQueryService;
        _teacherQueryService = teacherQueryService;
        _subjectQueryService = subjectQueryService;
        _context = context;
    }

    #region Public Methods

    public async Task<bool> CreateSchedulesAsync(CreateSchedulesRequestDto request)
    {
        // Validate all schedules before creating any
        foreach (var item in request.Schedules)
        {
            // Find matching TimeSlot
            var timeSlot = await FindTimeSlotByTimesAsync(item.StartTime, item.EndTime);

            // Validate room availability
            await ValidateNoRoomConflictAsync(item.RoomId, item.DayId, item.StartTime, item.EndTime);

            // Validate teacher availability
            await ValidateNoTeacherConflictAsync(item.TeacherId, item.DayId, item.StartTime, item.EndTime);
        }

        // All validations passed, create schedules
        foreach (var item in request.Schedules)
        {
            var timeSlot = await FindTimeSlotByTimesAsync(item.StartTime, item.EndTime);

            var schedule = new Schedule
            {
                Id = Guid.NewGuid(),
                GroupId = item.GroupId,
                DayId = item.DayId,
                TimeSlotId = timeSlot.Id,
                RoomId = item.RoomId,
                TeacherId = item.TeacherId,
                SubjectId = item.SubjectId
            };

            await _context.Schedules.AddAsync(schedule);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<GroupScheduleResponseDto> GetGroupScheduleAsync(Guid groupId)
    {
        var schedules = await _scheduleQueryService.GetSchedulesByGroupIdAsync(groupId);

        // Group by Day
        var groupedByDay = schedules
            .GroupBy(s => new { s.DayId, s.Day.Name, s.Day.OrderIndex })
            .OrderBy(g => g.Key.OrderIndex)
            .Select(dayGroup => new DayScheduleDto
            {
                DayId = dayGroup.Key.DayId,
                DayName = dayGroup.Key.Name,
                Sessions = dayGroup
                    .OrderBy(s => s.TimeSlot.StartTime)
                    .Select(s => new SessionDto
                    {
                        ScheduleId = s.Id,
                        TimeSlotId = s.TimeSlotId,
                        StartTime = s.TimeSlot.StartTime,
                        EndTime = s.TimeSlot.EndTime,
                        Room = new RoomInfoDto
                        {
                            Id = s.Room.Id,
                            Name = s.Room.Name
                        },
                        Teacher = new TeacherInfoDto
                        {
                            Id = s.Teacher.Id,
                            Name = $"{s.Teacher.FirstName} {s.Teacher.LastName}"
                        },
                        Subject = new SubjectInfoDto
                        {
                            Id = s.Subject.Id,
                            Name = s.Subject.Name
                        }
                    })
                    .ToList()
            })
            .ToList();

        return new GroupScheduleResponseDto
        {
            GroupId = groupId,
            Days = groupedByDay
        };
    }

    public async Task<bool> UpdateScheduleAsync(Guid scheduleId, UpdateScheduleRequestDto request)
    {
        // Find existing schedule
        var schedule = await _scheduleQueryService.GetByIdAsync(scheduleId);
        if (schedule == null)
        {
            throw new InvalidOperationException($"Schedule with ID {scheduleId} not found.");
        }

        // Find matching TimeSlot
        var timeSlot = await FindTimeSlotByTimesAsync(request.StartTime, request.EndTime);

        // Validate room availability (exclude current schedule)
        await ValidateNoRoomConflictAsync(request.RoomId, request.DayId, request.StartTime, request.EndTime, scheduleId);

        // Validate teacher availability (exclude current schedule)
        await ValidateNoTeacherConflictAsync(request.TeacherId, request.DayId, request.StartTime, request.EndTime, scheduleId);

        // Update schedule
        schedule.DayId = request.DayId;
        schedule.TimeSlotId = timeSlot.Id;
        schedule.RoomId = request.RoomId;
        schedule.TeacherId = request.TeacherId;
        schedule.SubjectId = request.SubjectId;

        _context.Schedules.Update(schedule);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteScheduleAsync(Guid scheduleId)
    {
        var schedule = await _scheduleQueryService.GetByIdAsync(scheduleId);
        if (schedule == null)
        {
            throw new InvalidOperationException($"Schedule with ID {scheduleId} not found.");
        }

        // Soft delete using EF shadow property
        _context.Entry(schedule).Property("DeletedAt").CurrentValue = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<RoomAvailabilityResponseDto> CheckRoomAvailabilityAsync(
        Guid roomId, 
        Guid dayId, 
        TimeOnly startTime, 
        TimeOnly endTime, 
        Guid? excludeScheduleId = null)
    {
        var existingSchedules = await _scheduleQueryService.GetRoomSchedulesAsync(roomId, dayId);

        // Exclude current schedule when updating
        if (excludeScheduleId.HasValue)
        {
            existingSchedules = existingSchedules.Where(s => s.Id != excludeScheduleId.Value).ToList();
        }

        // Check for conflicts
        var conflicts = new List<ConflictDetailDto>();
        foreach (var schedule in existingSchedules)
        {
            if (HasTimeOverlap(startTime, endTime, schedule.TimeSlot.StartTime, schedule.TimeSlot.EndTime))
            {
                conflicts.Add(new ConflictDetailDto
                {
                    ScheduleId = schedule.Id,
                    StartTime = schedule.TimeSlot.StartTime,
                    EndTime = schedule.TimeSlot.EndTime,
                    ConflictingResource = schedule.Group?.Subject?.Name ?? "Unknown Subject"
                });
            }
        }

        var room = await _roomQueryService.GetByIdAsync(roomId);
        var day = await _dayQueryService.GetByIdAsync(dayId);

        return new RoomAvailabilityResponseDto
        {
            Available = conflicts.Count == 0,
            RoomName = room?.Name ?? "Unknown Room",
            DayName = day?.Name ?? "Unknown Day",
            RequestedStartTime = startTime,
            RequestedEndTime = endTime,
            Conflicts = conflicts
        };
    }

    public async Task<TeacherAvailabilityResponseDto> CheckTeacherAvailabilityAsync(
        Guid teacherId, 
        Guid dayId, 
        TimeOnly startTime, 
        TimeOnly endTime, 
        Guid? excludeScheduleId = null)
    {
        var existingSchedules = await _scheduleQueryService.GetTeacherSchedulesAsync(teacherId, dayId);

        // Exclude current schedule when updating
        if (excludeScheduleId.HasValue)
        {
            existingSchedules = existingSchedules.Where(s => s.Id != excludeScheduleId.Value).ToList();
        }

        // Check for conflicts
        var conflicts = new List<ConflictDetailDto>();
        foreach (var schedule in existingSchedules)
        {
            if (HasTimeOverlap(startTime, endTime, schedule.TimeSlot.StartTime, schedule.TimeSlot.EndTime))
            {
                conflicts.Add(new ConflictDetailDto
                {
                    ScheduleId = schedule.Id,
                    StartTime = schedule.TimeSlot.StartTime,
                    EndTime = schedule.TimeSlot.EndTime,
                    ConflictingResource = schedule.Group?.Subject?.Name ?? "Unknown Subject"
                });
            }
        }

        var teacher = await _teacherQueryService.GetByIdAsync(teacherId);
        var day = await _dayQueryService.GetByIdAsync(dayId);

        return new TeacherAvailabilityResponseDto
        {
            Available = conflicts.Count == 0,
            TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "Unknown Teacher",
            DayName = day?.Name ?? "Unknown Day",
            RequestedStartTime = startTime,
            RequestedEndTime = endTime,
            Conflicts = conflicts
        };
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Finds a TimeSlot by matching StartTime and EndTime.
    /// Throws exception if not found.
    /// </summary>
    private async Task<TimeSlot> FindTimeSlotByTimesAsync(TimeOnly startTime, TimeOnly endTime)
    {
        var allTimeSlots = await _timeSlotQueryService.GetAllAsync();
        var timeSlot = allTimeSlots.FirstOrDefault(ts => 
            ts.StartTime == startTime && ts.EndTime == endTime);

        if (timeSlot == null)
        {
            throw new InvalidOperationException(
                $"No TimeSlot found matching StartTime={startTime} and EndTime={endTime}. " +
                "Please ensure the time range matches one of the pre-seeded TimeSlots.");
        }

        return timeSlot;
    }

    /// <summary>
    /// Validates that a room is available (no conflicts) for the given day and time range.
    /// Throws exception if conflict detected.
    /// </summary>
    private async Task ValidateNoRoomConflictAsync(
        Guid roomId, 
        Guid dayId, 
        TimeOnly startTime, 
        TimeOnly endTime, 
        Guid? excludeScheduleId = null)
    {
        var existingSchedules = await _scheduleQueryService.GetRoomSchedulesAsync(roomId, dayId);

        // Exclude current schedule when updating
        if (excludeScheduleId.HasValue)
        {
            existingSchedules = existingSchedules.Where(s => s.Id != excludeScheduleId.Value).ToList();
        }

        foreach (var schedule in existingSchedules)
        {
            if (HasTimeOverlap(startTime, endTime, schedule.TimeSlot.StartTime, schedule.TimeSlot.EndTime))
            {
                var room = await _roomQueryService.GetByIdAsync(roomId);
                var day = await _dayQueryService.GetByIdAsync(dayId);
                throw new InvalidOperationException(
                    $"Room conflict: '{room?.Name}' is already booked on {day?.Name} " +
                    $"from {schedule.TimeSlot.StartTime} to {schedule.TimeSlot.EndTime} " +
                    $"for {schedule.Group?.Subject?.Name}.");
            }
        }
    }

    /// <summary>
    /// Validates that a teacher is available (no conflicts) for the given day and time range.
    /// Throws exception if conflict detected.
    /// </summary>
    private async Task ValidateNoTeacherConflictAsync(
        Guid teacherId, 
        Guid dayId, 
        TimeOnly startTime, 
        TimeOnly endTime, 
        Guid? excludeScheduleId = null)
    {
        var existingSchedules = await _scheduleQueryService.GetTeacherSchedulesAsync(teacherId, dayId);

        // Exclude current schedule when updating
        if (excludeScheduleId.HasValue)
        {
            existingSchedules = existingSchedules.Where(s => s.Id != excludeScheduleId.Value).ToList();
        }

        foreach (var schedule in existingSchedules)
        {
            if (HasTimeOverlap(startTime, endTime, schedule.TimeSlot.StartTime, schedule.TimeSlot.EndTime))
            {
                var teacher = await _teacherQueryService.GetByIdAsync(teacherId);
                var day = await _dayQueryService.GetByIdAsync(dayId);
                throw new InvalidOperationException(
                    $"Teacher conflict: '{teacher?.FirstName} {teacher?.LastName}' is already scheduled on {day?.Name} " +
                    $"from {schedule.TimeSlot.StartTime} to {schedule.TimeSlot.EndTime} " +
                    $"for {schedule.Group?.Subject?.Name}.");
            }
        }
    }

    /// <summary>
    /// Detects time overlap using the standard interval overlap algorithm:
    /// (start1 < end2) AND (end1 > start2)
    /// </summary>
    private bool HasTimeOverlap(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
    {
        return start1 < end2 && end1 > start2;
    }

    #endregion
}
