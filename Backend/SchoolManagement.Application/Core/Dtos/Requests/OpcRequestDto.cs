using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class OpcRequestDto
{
    [Required, MinLength(2), MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(150)]
    public string Slug { get; set; } = string.Empty;

    public Guid? GenderId { get; set; }

    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }

    [Required, Phone, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public DateTime? HireDate { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Salary must be non-negative.")]
    public decimal Salary { get; set; }
}
