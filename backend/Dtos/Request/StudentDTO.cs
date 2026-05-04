using System.ComponentModel.DataAnnotations;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Dtos;

public class StudentDto
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

    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Gender")]
    public int? GenderId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Level")]
    public int LevelId { get; set; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "Please select a valid Group")]
    public int GroupId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Intake")]
    public int? IntakeId { get; set; }
}