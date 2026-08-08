using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Academic.Dtos.Commands;

public record UpdateTeacherCommand
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
   
    public string Phone { get; set; } = string.Empty;


    public Guid GenderId { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    public DateTime HireDate { get; set; }

    
    public string Specialization { get; set; } = string.Empty;
    public decimal Salary { get; init; }
}
