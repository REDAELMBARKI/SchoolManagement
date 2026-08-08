using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

public class PlatformRequestDto
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
