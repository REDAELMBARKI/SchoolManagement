namespace SchoolManagement.Application.Core.Dtos.Commands;

public class UpdateCommercialAgentCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}
