using MediatR;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;

namespace SchoolManagement.Domain.DomainEvents.Students;

public class NewStudentAssignedDomainEvent : INotification
{
    public Guid StudentId { get; }
    public Guid EnrollmentId  { get; }
    public DateTime ChangedAt { get; }

    public NewStudentAssignedDomainEvent(Guid studentId ,  Guid enrollmentId)
    {
        StudentId = studentId;
        EnrollmentId = enrollmentId;
        ChangedAt = DateTime.UtcNow;
    }
}

