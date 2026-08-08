using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Academic.Dtos.Requests;

public class UpdateSubjectRequestDto
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
}
