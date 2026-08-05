using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

public class EnrollmentGroupTransferredDomainEvent : INotification
{
    public Guid EnrollmentId { get; }
    public Guid StudentId { get; }
    public Guid OldGroupId { get; }
    public Guid NewGroupId { get; }
    public string Reason { get; }
    public DateTime TransferredAt { get; }

    public EnrollmentGroupTransferredDomainEvent(
        Guid enrollmentId,
        Guid studentId,
        Guid oldGroupId,
        Guid newGroupId,
        string reason,
        DateTime transferredAt)
    {
        EnrollmentId = enrollmentId;
        StudentId = studentId;
        OldGroupId = oldGroupId;
        NewGroupId = newGroupId;
        Reason = reason;
        TransferredAt = transferredAt;
    }
}
