namespace SchoolManagement.Application.Common.Dtos.Requests;

public class ConvertToStaffRequestDto
{
    public string Role { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
}
