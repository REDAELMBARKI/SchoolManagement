using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

public class EnrollmentDroppedDomainEvent : INotification
{
    public Guid EnrollmentId { get; }
    public Guid GroupId { get; }
    public string Reason { get; }
    public DateTime DroppedAt { get; }

    public EnrollmentDroppedDomainEvent(Guid enrollmentId, Guid groupId, string reason, DateTime droppedAt)
    {
        EnrollmentId = enrollmentId;
        GroupId = groupId;
        Reason = reason;
        DroppedAt = droppedAt;
    }
}
