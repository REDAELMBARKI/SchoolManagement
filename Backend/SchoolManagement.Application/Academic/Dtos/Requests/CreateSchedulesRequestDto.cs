using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Academic.Dtos.Requests;

/// <summary>
/// Request to create multiple schedules for a group in one operation.
/// </summary>
public class CreateSchedulesRequestDto
{
    [Required]
    public List<ScheduleItemRequestDto> Schedules { get; set; } = new();
}

/// <summary>
/// Individual schedule item within bulk creation request.
/// </summary>
public class ScheduleItemRequestDto
{
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public Guid DayId { get; set; }

    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public Guid TeacherId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }
}
