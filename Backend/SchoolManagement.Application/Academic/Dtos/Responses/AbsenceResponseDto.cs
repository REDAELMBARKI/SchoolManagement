namespace SchoolManagement.Application.Academic.Dtos.Responses;

public class AbsenceResponseDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid BranchId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Absent";
    public bool IsJustified { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
