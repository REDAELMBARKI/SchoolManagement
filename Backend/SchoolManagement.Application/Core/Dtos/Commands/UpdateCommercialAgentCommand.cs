namespace SchoolManagement.Application.Core.Dtos.Commands;

public record UpdateCommercialAgentCommand
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string Phone { get; init; } = string.Empty;
    public decimal Salary { get; init; }
}
