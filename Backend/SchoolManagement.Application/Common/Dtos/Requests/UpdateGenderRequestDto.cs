using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

public class UpdateGenderRequestDto
{
    [Required, MinLength(2), MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(50)]
    public string Slug { get; set; } = string.Empty;
}
