namespace SchoolManagement.Application.Academic.Dtos.Responses;

/// <summary>
/// Response containing all schedules for a group, grouped by day.
/// </summary>
public class GroupScheduleResponseDto
{
    public Guid GroupId { get; set; }
    public List<DayScheduleDto> Days { get; set; } = new();
}

/// <summary>
/// Schedules for a specific day.
/// </summary>
public class DayScheduleDto
{
    public Guid DayId { get; set; }
    public string DayName { get; set; } = string.Empty;
    public List<SessionDto> Sessions { get; set; } = new();
}

/// <summary>
/// Individual session (schedule) details.
/// </summary>
public class SessionDto
{
    public Guid ScheduleId { get; set; }
    public Guid TimeSlotId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public RoomInfoDto Room { get; set; } = null!;
    public TeacherInfoDto Teacher { get; set; } = null!;
    public SubjectInfoDto Subject { get; set; } = null!;
}

/// <summary>
/// Simplified room info for schedule display.
/// </summary>
public class RoomInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Simplified teacher info for schedule display.
/// </summary>
public class TeacherInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Simplified subject info for schedule display.
/// </summary>
public class SubjectInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
