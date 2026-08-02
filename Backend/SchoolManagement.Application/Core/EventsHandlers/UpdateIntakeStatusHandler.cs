using MediatR;
using SchoolManagement.Domain.Core.DomainEvents;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Core.EventsHandlers
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
