using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Commands;

public class StudentCommand
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public Guid? GenderId { get; set; }

    public Guid LevelId { get; set; }

    public Guid? IntakeId { get; set; }
    public bool IsDirectRegistration { get; set; }

    public string Slug { get; set; } = string.Empty;
}
