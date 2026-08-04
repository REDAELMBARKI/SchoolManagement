namespace SchoolManagement.Application.Academic.Dtos.Responses;

/// <summary>
/// Response for teacher availability check (AJAX validation).
/// </summary>
public class TeacherAvailabilityResponseDto
{
    public bool Available { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public TimeOnly RequestedStartTime { get; set; }
    public TimeOnly RequestedEndTime { get; set; }
    public List<ConflictDetailDto> Conflicts { get; set; } = new();
}

