using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

public class AddClaimRequestDto
{
    [Required]
    public string ClaimType { get; set; } = string.Empty;

    [Required]
    public string ClaimValue { get; set; } = string.Empty;
}
