using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

public class ConfirmEmailRequestDto
{
    [Required]
    public string ApplicationUserId { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}
