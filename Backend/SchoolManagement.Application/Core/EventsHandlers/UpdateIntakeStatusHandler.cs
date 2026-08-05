using MediatR;
using SchoolManagement.Domain.Core.DomainEvents;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;

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
