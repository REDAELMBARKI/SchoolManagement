namespace SchoolManagement.Application.Academic.Dtos.Requests;

public record CreateAbsenceRequestDto
{
    public Guid StudentId { get; init; }
    public Guid ScheduleId { get; init; }
    public DateTime? Date { get; init; }
    public string Status { get; init; } = "Absent";
    public bool IsJustified { get; init; }
    public string? Reason { get; init; }
}
