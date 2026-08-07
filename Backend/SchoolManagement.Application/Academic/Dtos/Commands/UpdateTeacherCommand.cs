namespace SchoolManagement.Application.Academic.Dtos.Commands;

public record UpdateTeacherCommand
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string Phone { get; init; } = string.Empty;
    public decimal Salary { get; init; }
    public string Specialization { get; init; } = string.Empty;
}
