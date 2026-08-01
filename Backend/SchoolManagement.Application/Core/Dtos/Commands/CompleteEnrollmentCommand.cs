namespace SchoolManagement.Application.Core.Dtos.Commands;

public class CompleteEnrollmentCommand
{
    public Guid EnrollmentId { get; set; }
    public string? Notes { get; set; }
    public Guid CompletedByUserId { get; set; }
}
