using MediatR;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Events.Students;

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

