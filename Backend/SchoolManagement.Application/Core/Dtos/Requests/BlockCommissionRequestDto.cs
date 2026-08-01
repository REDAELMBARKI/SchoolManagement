using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class BlockCommissionRequestDto
{
    [Required, MinLength(3), MaxLength(300)]
    public string Reason { get; set; } = string.Empty;
}
