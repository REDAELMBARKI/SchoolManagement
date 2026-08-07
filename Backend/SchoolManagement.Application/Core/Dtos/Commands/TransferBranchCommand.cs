namespace SchoolManagement.Application.Core.Dtos.Commands;

public class TransferBranchCommand
{
    public Guid StudentId { get; set; }
    public Guid NewBranchId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
