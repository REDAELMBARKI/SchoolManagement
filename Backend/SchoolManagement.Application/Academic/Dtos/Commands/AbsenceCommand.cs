namespace SchoolManagement.Application.Academic.Dtos.Commands;

public record AbsenceCommand
{
    public Guid StudentId { get; init; }
    public Guid ScheduleId { get; init; }
    public Guid BranchId { get; init; }
    public DateTime? Date { get; init; }
    public string Status { get; init; } = "Absent";
    public bool IsJustified { get; init; }
    public string? Reason { get; init; }
}
