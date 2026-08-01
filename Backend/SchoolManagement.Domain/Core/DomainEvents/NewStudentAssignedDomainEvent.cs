using MediatR;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Domain.Core.DomainEvents;

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

