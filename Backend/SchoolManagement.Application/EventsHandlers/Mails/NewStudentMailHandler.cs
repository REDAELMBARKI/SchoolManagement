using MediatR;
using SchoolManagement.Application.Events.Students;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Handlers.Mails;
    public class NewStudentMailHandler : INotificationHandler<NewStudentAsignedEvent>
    {
        public Task Handle(NewStudentAsignedEvent e, CancellationToken cancellationToken)
        {
            Console.WriteLine($"emailing to the admin new student created: {e.Student.LastName}"); 
            return Task.CompletedTask;
        }
    }

