using MediatR;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Events.Students;

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


