using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Academic.Dtos.Requests;

/// <summary>
/// Request to update an existing schedule.
/// </summary>
public class UpdateScheduleRequestDto
{
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
