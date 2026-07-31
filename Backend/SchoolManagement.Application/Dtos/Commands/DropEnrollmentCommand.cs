namespace SchoolManagement.Application.Dtos.Commands;

public class DropEnrollmentCommand
{
    public Guid EnrollmentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid DroppedByUserId { get; set; }
}
