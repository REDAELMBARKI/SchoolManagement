using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

public class CreateStaffUserRequestDto
{
    // Authentication fields
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;  // SuperAdmin, Director, Administrator, Receptionist, Teacher, CommercialAgent

    // Domain/Business fields
    [Required, MinLength(2), MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
 
    [Required, MinLength(2), MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Phone, MaxLength(20)]
    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }
 
    public Guid? GenderId { get; set; }

    public Guid? BranchId { get; set; }  // Required for non-SuperAdmin roles
}
