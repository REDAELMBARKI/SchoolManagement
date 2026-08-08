using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Academic.Dtos.Requests;

public class TeacherRequestDto
{
    [Required, MinLength(2), MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required, MinLength(2), MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    public Guid? GenderId { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    [Required]
    public string Phone { get; set; } = string.Empty;
    
    public DateOnly? DateOfBirth { get; set; }
    
    public DateTime HireDate { get; set; }
    
    [MaxLength(200)]
    [Required]
    public string Specialization { get; set; } = string.Empty;
    [Range(0, double.MaxValue)]
    public decimal Salary { get; set; }

}
