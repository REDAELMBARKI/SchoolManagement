using MediatR;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Domain.DomainEvents.Students;

public class NewStudentAsignedDomainEvent : INotification
{
    public Student Student { get; }
    public DateTime ChangedAt { get; }

    public NewStudentAsignedDomainEvent(Student student)
    {
        Student = student;
        ChangedAt = DateTime.UtcNow;
    }
}

