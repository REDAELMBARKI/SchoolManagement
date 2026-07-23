using MediatR;

namespace SchoolManagement.Domain.DomainEvents.Students;

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
