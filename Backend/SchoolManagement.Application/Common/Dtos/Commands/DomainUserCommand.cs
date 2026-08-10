namespace SchoolManagement.Application.Common.Dtos.Commands;

public class DomainUserCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Guid? GenderId { get; set; }
    public string Role { get; set; } = string.Empty;
    public Guid BranchId { get; set; }  // REQUIRED - All staff must have a branch (NO SuperAdmin creation via API)
    public string ApplicationUserId { get; set; } = string.Empty; 
}
