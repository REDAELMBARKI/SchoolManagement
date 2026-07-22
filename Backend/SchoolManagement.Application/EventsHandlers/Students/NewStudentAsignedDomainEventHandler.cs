using MediatR;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.EventsHandlers.Students
{
    public class NewStudentAsignedDomainEventHandler : INotificationHandler<NewStudentAsignedDomainEvent>
    {
        public Task Handle(NewStudentAsignedDomainEvent DomainEvent, CancellationToken cancellationToken)
        {
            Console.WriteLine($"New student assigned: {DomainEvent.Student.FirstName} {DomainEvent.Student.LastName} at {DomainEvent.ChangedAt}");
            Intake? intake  = DomainEvent.Student.Intake;
            if (intake != null)
            {
                ChangeIntakeStatus(intake);
                AsignStudentToConvertedIntake(intake, DomainEvent.Student.Id);
            }
            return Task.CompletedTask;
        }


        private void SendEmailNotification(NewStudentAsignedDomainEvent DomainEvent)
        {
            // Implement email sending logic here
            Console.WriteLine($"Sending email notification for new student: {DomainEvent.Student.FirstName} {DomainEvent.Student.LastName}");
        }



        private void ChangeIntakeStatus(Intake intake) { 
             intake.MarkAsEnrolled
        }
        
        private void AsignStudentToConvertedIntake(Intake intake , Guid StudentId) { }
    }
}
