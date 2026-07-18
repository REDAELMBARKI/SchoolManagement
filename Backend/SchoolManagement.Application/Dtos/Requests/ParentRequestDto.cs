namespace SchoolManagement.Application.Dtos.Requests;

public class ParentRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? GenderId { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
}

