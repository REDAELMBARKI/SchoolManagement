namespace SchoolManagement.Application.Core.Dtos.Commands;

public class TransferGroupCommand
{
    public Guid EnrollmentId { get; set; }
    public Guid NewGroupId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
