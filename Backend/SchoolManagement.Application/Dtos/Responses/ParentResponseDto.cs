namespace SchoolManagement.Application.Dtos.Responses;

public class ParentResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Relationship { get; set; } = string.Empty; 
    public GenderResponseDto? Gender { get; set; }
}
