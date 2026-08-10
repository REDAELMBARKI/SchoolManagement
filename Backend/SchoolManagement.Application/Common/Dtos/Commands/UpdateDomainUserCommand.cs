namespace SchoolManagement.Application.Common.Dtos.Commands;

public class UpdateDomainUserCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Guid? GenderId { get; set; }
}
