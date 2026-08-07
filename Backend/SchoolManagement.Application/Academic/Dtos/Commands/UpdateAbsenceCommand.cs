namespace SchoolManagement.Application.Academic.Dtos.Commands;

public record UpdateAbsenceCommand
{
    public string Status { get; init; } = "Absent";
    public bool IsJustified { get; init; }
    public string? Reason { get; init; }
}
