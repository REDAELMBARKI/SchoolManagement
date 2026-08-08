using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Academic.Dtos.Commands;

public class UpdateTeacherCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Guid GenderId { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}
