using MediatR;
using SchoolManagement.Backend.Events.Students;

namespace SchoolManagement.Backend.Handlers.Mails
{
    public class IntakeConvertedToStudentMailHandler : INotificationHandler<IntakeConvertedToStudentEvent>
    {

        public Task Handle(IntakeConvertedToStudentEvent e, CancellationToken cancellationToken)
        {
            Console.WriteLine($"emailing to the admin intake converted to student: {e.Student.LastName}");
            return Task.CompletedTask;
        }
    }
}
