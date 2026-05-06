namespace SchoolManagement.Backend.Dtos.Responses;

public class ParentResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    
    // Inherited from Person
    public GenderResponseDto Gender { get; set; } = null!;
}
