using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Application.Academic.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _repository;
    private readonly IScheduleQueryService _scheduleQueryService;
    private readonly ITimeSlotQueryService _timeSlotQueryService;
    private readonly IDayQueryService _dayQueryService;
    private readonly IRoomQueryService _roomQueryService;
    private readonly ITeacherQueryService _teacherQueryService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;
    private readonly ITransaction _transaction;

    public ScheduleService(
        IScheduleRepository repository,
        IScheduleQueryService scheduleQueryService,
        ITimeSlotQueryService timeSlotQueryService,
        IDayQueryService dayQueryService,
        IRoomQueryService roomQueryService,
        ITeacherQueryService teacherQueryService,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService,
        ITransaction transaction)
    {
        _repository = repository;
        _scheduleQueryService = scheduleQueryService;
        _timeSlotQueryService = timeSlotQueryService;
        _dayQueryService = dayQueryService;
        _roomQueryService = roomQueryService;
        _teacherQueryService = teacherQueryService;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
        _transaction = transaction;
    }

    #region Public Methods

    public async Task<bool> CreateSchedulesAsync(CreateSchedulesRequestDto request)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        await _transaction.BeginTransactionAsync();
        try
        {
            // Validate all schedules before creating any (two-phase validation)
            foreach (var item in request.Schedules)
            {
                var timeSlot = await FindTimeSlotByTimesAsync(item.StartTime, item.EndTime);
                await ValidateNoRoomConflictAsync(item.RoomId, item.DayId, item.StartTime, item.EndTime);
                await ValidateNoTeacherConflictAsync(item.TeacherId, item.DayId, item.StartTime, item.EndTime);
            }

            // All validations passed, create schedules using domain factory
            foreach (var item in request.Schedules)
            {
                var timeSlot = await FindTimeSlotByTimesAsync(item.StartTime, item.EndTime);

                var command = new ScheduleCommand
                {
                    BranchId = branchId,
                    TeacherId = item.TeacherId,
                    RoomId = item.RoomId,
                    DayId = item.DayId,
                    TimeSlotId = timeSlot.Id,
                    GroupId = item.GroupId,
                    SubjectId = item.SubjectId
                };

                var schedule = ScheduleMapper.ToDomain(command);
                await _repository.AddAsync(schedule);

                await _auditLogService.StoreAsync(
                    action: AuditLog.CreateAction(),
                    entityName: nameof(Schedule),
                    entityId: schedule.Id,
                    branchId: branchId,
                    newValues: new { schedule.GroupId, schedule.DayId, schedule.TimeSlotId, 
                                    schedule.RoomId, schedule.TeacherId, schedule.SubjectId });
            }

            await _transaction.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _transaction.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<GroupScheduleResponseDto> GetGroupScheduleAsync(Guid groupId)
    {
        var schedules = await _scheduleQueryService.GetSchedulesByGroupIdAsync(groupId);

        var groupedByDay = schedules
            .GroupBy(s => s.Day.Name )
            .OrderBy(g => g.Key)
            .Select(dayGroup => new DayScheduleDto
            {
                //DayId = dayGroup.Key.DayId,
                //DayName = dayGroup.Key.Name,
                Sessions = dayGroup
                    .OrderBy(s => s.TimeSlot.StartTime)
                    .Select(s => new SessionDto
                    {
                        ScheduleId = s.Id,
                        TimeSlotId = s.TimeSlotId,
                        StartTime = s.TimeSlot.StartTime,
                        EndTime = s.TimeSlot.EndTime,
                        Room = new RoomInfoDto { Id = s.Room.Id, Name = s.Room.Name },
                        Teacher = new TeacherInfoDto { Id = s.Teacher.Id, 
                                   Name = $"{s.Teacher.FirstName} {s.Teacher.LastName}" },
                        Subject = new SubjectInfoDto { Id = s.Subject.Id, Name = s.Subject.Name }
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
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        await _transaction.BeginTransactionAsync();
        try
        {
            var schedule = await _repository.GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new NotFoundException($"Schedule with ID {scheduleId} not found.");

            var timeSlot = await FindTimeSlotByTimesAsync(request.StartTime, request.EndTime);
            await ValidateNoRoomConflictAsync(request.RoomId, request.DayId, request.StartTime, request.EndTime, scheduleId);
            await ValidateNoTeacherConflictAsync(request.TeacherId, request.DayId, request.StartTime, request.EndTime, scheduleId);

            var oldValues = new { schedule.DayId, schedule.TimeSlotId, schedule.RoomId, 
                                 schedule.TeacherId, schedule.SubjectId };

            // Update using domain methods
            schedule.UpdateDayId(request.DayId);
            schedule.UpdateTimeSlotId(timeSlot.Id);
            schedule.UpdateRoomId(request.RoomId);
            schedule.UpdateTeacherId(request.TeacherId);
            schedule.UpdateSubjectId(request.SubjectId);

            await _repository.UpdateAsync(schedule);

            await _auditLogService.StoreAsync(
                action: AuditLog.UpdateAction(),
                entityName: nameof(Schedule),
                entityId: schedule.Id,
                branchId: branchId,
                oldValues: oldValues,
                newValues: new { schedule.DayId, schedule.TimeSlotId, schedule.RoomId, 
                                schedule.TeacherId, schedule.SubjectId });

            await _transaction.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _transaction.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> DeleteScheduleAsync(Guid scheduleId)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        await _transaction.BeginTransactionAsync();
        try
        {
            var schedule = await _repository.GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new NotFoundException($"Schedule with ID {scheduleId} not found.");

            await _repository.DeleteAsync(scheduleId);

            await _auditLogService.StoreAsync(
                action: AuditLog.DeleteAction(),
                entityName: nameof(Schedule),
                entityId: scheduleId,
                branchId: branchId,
                oldValues: new { schedule.GroupId, schedule.DayId, schedule.TimeSlotId, 
                                schedule.RoomId, schedule.TeacherId, schedule.SubjectId });

            await _transaction.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _transaction.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<RoomAvailabilityResponseDto> CheckRoomAvailabilityAsync(
        Guid roomId, Guid dayId, TimeOnly startTime, TimeOnly endTime, Guid? excludeScheduleId = null)
    {
        var existingSchedules = await _scheduleQueryService.GetRoomSchedulesAsync(roomId, dayId);

        if (excludeScheduleId.HasValue)
            existingSchedules = existingSchedules.Where(s => s.Id != excludeScheduleId.Value).ToList();

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
        Guid teacherId, Guid dayId, TimeOnly startTime, TimeOnly endTime, Guid? excludeScheduleId = null)
    {
        var existingSchedules = await _scheduleQueryService.GetTeacherSchedulesAsync(teacherId, dayId);

        if (excludeScheduleId.HasValue)
            existingSchedules = existingSchedules.Where(s => s.Id != excludeScheduleId.Value).ToList();

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

    private async Task<TimeSlot> FindTimeSlotByTimesAsync(TimeOnly startTime, TimeOnly endTime)
    {
        var timeSlot = await _timeSlotQueryService.GetByTimesAsync(startTime, endTime);

        if (timeSlot == null)
        {
            throw new NotFoundException(
                $"No TimeSlot found matching StartTime={startTime} and EndTime={endTime}. " +
                "Please ensure the time range matches one of the pre-seeded TimeSlots.");
        }

        return timeSlot;
    }

    private async Task ValidateNoRoomConflictAsync(
        Guid roomId, Guid dayId, TimeOnly startTime, TimeOnly endTime, Guid? excludeScheduleId = null)
    {
        var existingSchedules = await _scheduleQueryService.GetRoomSchedulesAsync(roomId, dayId);

        if (excludeScheduleId.HasValue)
            existingSchedules = existingSchedules.Where(s => s.Id != excludeScheduleId.Value).ToList();

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

    private async Task ValidateNoTeacherConflictAsync(
        Guid teacherId, Guid dayId, TimeOnly startTime, TimeOnly endTime, Guid? excludeScheduleId = null)
    {
        var existingSchedules = await _scheduleQueryService.GetTeacherSchedulesAsync(teacherId, dayId);

        if (excludeScheduleId.HasValue)
            existingSchedules = existingSchedules.Where(s => s.Id != excludeScheduleId.Value).ToList();

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

    private bool HasTimeOverlap(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
    {
        return start1 < end2 && end1 > start2;
    }

    #endregion
}
