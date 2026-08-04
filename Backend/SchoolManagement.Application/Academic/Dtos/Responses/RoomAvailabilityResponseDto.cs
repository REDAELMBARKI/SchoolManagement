namespace SchoolManagement.Application.Academic.Dtos.Responses;

/// <summary>
/// Response for room availability check (AJAX validation).
/// </summary>
public class RoomAvailabilityResponseDto
{
    public bool Available { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public TimeOnly RequestedStartTime { get; set; }
    public TimeOnly RequestedEndTime { get; set; }
    public List<ConflictDetailDto> Conflicts { get; set; } = new();
}

public class ConflictDetailDto
{
    public Guid ScheduleId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string ConflictingResource { get; set; } = string.Empty;
}

