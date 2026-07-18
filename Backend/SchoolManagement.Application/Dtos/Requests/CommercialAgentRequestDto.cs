namespace SchoolManagement.Application.Dtos.Requests;

public class CommercialAgentRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? GenderId { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public Guid BranchId { get; set; }
}

