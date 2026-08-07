namespace SchoolManagement.Application.Academic.Dtos.Responses;

public class ScheduleConflictResponseDto
{
    public bool HasConflict { get; set; }
    public List<ConflictDetailDto> Conflicts { get; set; } = new();
}

public class ConflictDetailDto
{
    public string DayName { get; set; } = string.Empty;
    public string ExistingTimeSlot { get; set; } = string.Empty;
    public string NewTimeSlot { get; set; } = string.Empty;
    public string ExistingSubjectName { get; set; } = string.Empty;
    public string NewSubjectName { get; set; } = string.Empty;
    public Guid ConflictingEnrollmentId { get; set; }
}
