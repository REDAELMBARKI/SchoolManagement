using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

public class EnrollmentCompletedDomainEvent : INotification
{
    public Guid EnrollmentId { get; }
    public Guid GroupId { get; }
    public DateTime CompletedAt { get; }
    public string? Notes { get; }

    public EnrollmentCompletedDomainEvent(Guid enrollmentId, Guid groupId, DateTime completedAt, string? notes)
    {
        EnrollmentId = enrollmentId;
        GroupId = groupId;
        CompletedAt = completedAt;
        Notes = notes;
    }
}
