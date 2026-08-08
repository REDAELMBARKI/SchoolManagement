namespace SchoolManagement.Application.Academic.Dtos.Requests;

public record UpdateAbsenceRequestDto
{
    public string Status { get; init; } = "Absent";
    public bool IsJustified { get; init; }
    public string? Reason { get; init; }
}
