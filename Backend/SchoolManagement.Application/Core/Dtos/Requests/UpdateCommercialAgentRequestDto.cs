namespace SchoolManagement.Application.Core.Dtos.Requests;

public record UpdateCommercialAgentRequestDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string Phone { get; init; } = string.Empty;
    public decimal Salary { get; init; }
}
