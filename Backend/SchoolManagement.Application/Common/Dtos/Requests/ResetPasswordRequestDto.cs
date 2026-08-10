using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

public class ResetPasswordRequestDto
{
    [Required, MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}
