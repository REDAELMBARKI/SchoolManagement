namespace SchoolManagement.Application.Academic.Dtos.Responses;

public class TeacherResponseDto
{
    public Guid Id { get; set; }
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
    public string Specialization { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation Properties
    public GenderResponseDto Gender { get; set; } = null!;
    public ICollection<GroupTeacherResponseDto> GroupTeachers { get; set; } = new List<GroupTeacherResponseDto>();
}
