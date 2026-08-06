namespace SchoolManagement.Application.Core.Dtos.Commands;

public class OpcCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? GenderId { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public decimal Salary { get; set; }
    
    // Populated by service from current user context
    public Guid BranchId { get; set; }
}
