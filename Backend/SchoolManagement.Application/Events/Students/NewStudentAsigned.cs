using MediatR;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Events.Students;
public class NewStudentAsignedEvent : INotification
{
    public Student Student { get; }
    public DateTime ChangedAt { get; }

    public NewStudentAsignedEvent(Student student)
    {
        Student = student;
        ChangedAt = DateTime.UtcNow;
    }
}

