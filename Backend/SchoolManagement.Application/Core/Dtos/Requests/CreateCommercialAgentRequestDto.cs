namespace SchoolManagement.Application.Core.Dtos.Requests;

public record CreateCommercialAgentRequestDto
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
}
