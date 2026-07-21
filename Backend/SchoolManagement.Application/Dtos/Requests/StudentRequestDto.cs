using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Dtos.Requests;


public class StudentRequestDto
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress, MaxLength(255)]
    public string? Email { get; set; } = string.Empty;

    [Required, Phone, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }

    public Guid? GenderId { get; set; }

    [Required]
    public Guid LevelId { get; set; }

    [Required]
    public Guid GroupId { get; set; }

    public Guid? IntakeId { get; set; }
}