namespace SchoolManagement.Application.Common.Dtos.Commands;

public class ConvertToStaffCommand
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
}
