using MediatR;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.EventsHandlers.Students
{
    public class UpdateIntakeStatusHandler : INotificationHandler<NewStudentAssignedDomainEvent>
    {
        IIntakeRepository _intakeRepository; 
        public UpdateIntakeStatusHandler(IIntakeRepository intakeRepository)
        {
            _intakeRepository = intakeRepository; 
        }

        public async Task Handle(NewStudentAssignedDomainEvent DomainEvent, CancellationToken cancellationToken)
        {
            Intake? intake  = await _intakeRepository.GetIntakeByStudentId(DomainEvent.StudentId);
            if (intake != null)
            {
                intake.MarkAsEnrolled();
                await _intakeRepository.SaveChangesAsync(cancellationToken);
            }
        }

    }
}
