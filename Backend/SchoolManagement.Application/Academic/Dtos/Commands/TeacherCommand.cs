namespace SchoolManagement.Application.Academic.Dtos.Commands;

public record TeacherCommand
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public Guid? GenderId { get; init; }
    public string? Email { get; init; }
    public string Phone { get; init; } = string.Empty;
    public DateOnly? DateOfBirth { get; init; }
    public DateTime HireDate { get; init; }
    public decimal Salary { get; init; }
    public Guid BranchId { get; init; }
    public string Specialization { get; init; } = string.Empty;
}
