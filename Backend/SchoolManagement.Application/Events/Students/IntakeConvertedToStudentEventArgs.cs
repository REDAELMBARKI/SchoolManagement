using MediatR;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Events.Students;

public class IntakeConvertedToStudentEvent : INotification
{
    public Intake Intake { get; }
    public Student Student { get; }
    public DateTime ConvertedAt { get; }

    public IntakeConvertedToStudentEvent(Student student)
    {
        Intake = student.Intake;
        Student = student;
        ConvertedAt = DateTime.UtcNow;
    }
}


