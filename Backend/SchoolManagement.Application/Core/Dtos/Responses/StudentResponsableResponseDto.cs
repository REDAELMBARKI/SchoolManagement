namespace SchoolManagement.Application.Core.Dtos.Responses;

public class StudentResponsableResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
}
