using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class UpdateAdRequestDto
{
    [Required, MinLength(2), MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid PlatformId { get; set; }

    [Required]
    public Guid BranchId { get; set; }
}
