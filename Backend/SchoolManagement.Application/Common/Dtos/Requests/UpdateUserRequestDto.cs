using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

public class UpdateUserRequestDto
{
    [Required, MinLength(2), MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
 
    [Required, MinLength(2), MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Phone, MaxLength(20)]
    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }
 
    public Guid? GenderId { get; set; }
}
