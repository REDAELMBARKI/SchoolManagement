using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

public class EnrollmentCreatedDomainEvent : INotification
{
    public Guid EnrollmentId { get; }
    public Guid StudentId { get; }
    public Guid BranchId { get; }
    public DateTime EnrolledAt { get; }

    public EnrollmentCreatedDomainEvent(Guid enrollmentId, Guid studentId, Guid branchId, DateTime enrolledAt)
    {
        EnrollmentId = enrollmentId;
        StudentId = studentId;
        BranchId = branchId;
        EnrolledAt = enrolledAt;
    }
}
