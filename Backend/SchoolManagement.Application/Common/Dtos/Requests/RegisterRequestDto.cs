using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

/// <summary>
/// DTO for public user registration (students/parents)
/// Only requires email and password
/// </summary>
public class RegisterRequestDto
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
