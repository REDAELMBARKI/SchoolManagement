using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

public class StudentCreatedDomainEvent : INotification
{
    public Guid StudentId { get; }
    public DateTime CreatedAt { get; }

    public StudentCreatedDomainEvent(Guid studentId)
    {
        StudentId = studentId;
        CreatedAt = DateTime.UtcNow;
    }
}
