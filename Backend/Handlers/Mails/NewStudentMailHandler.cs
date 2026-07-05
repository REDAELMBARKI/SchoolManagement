using MediatR;
using SchoolManagement.Backend.Events.Students;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Handlers.Mails
{
    public class NewStudentMailHandler : INotificationHandler<NewStudentAsignedEvent>
    {
        public Task Handle(NewStudentAsignedEvent e, CancellationToken cancellationToken)
        {
            Console.WriteLine($"emailing to the admin new student created: {e.Student.LastName}"); 
            return Task.CompletedTask;
        }
    }
}
