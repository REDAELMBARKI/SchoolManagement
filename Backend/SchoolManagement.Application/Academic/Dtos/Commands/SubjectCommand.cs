using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Academic.Dtos.Commands;

public class SubjectCommand
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
