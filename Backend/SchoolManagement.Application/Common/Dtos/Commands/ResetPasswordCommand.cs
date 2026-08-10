namespace SchoolManagement.Application.Common.Dtos.Commands;

public class ResetPasswordCommand
{
    public string NewPassword { get; set; } = string.Empty;
}
