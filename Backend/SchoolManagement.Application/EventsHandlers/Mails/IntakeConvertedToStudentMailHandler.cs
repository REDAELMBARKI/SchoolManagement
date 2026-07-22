using MediatR;
using SchoolManagement.Application.Events.Students;

namespace SchoolManagement.Application.Handlers.Mails
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
