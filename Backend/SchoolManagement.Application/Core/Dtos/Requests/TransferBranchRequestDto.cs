namespace SchoolManagement.Application.Core.Dtos.Requests;

public class TransferBranchRequestDto
{
    public Guid NewBranchId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
