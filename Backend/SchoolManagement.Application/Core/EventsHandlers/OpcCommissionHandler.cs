using MediatR;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Core.DomainEvents;

namespace SchoolManagement.Application.Core.EventsHandlers;

/// <summary>
/// Listens for EnrollmentCreatedDomainEvent and creates an OPC commission
/// when the enrolled student came from an OPC-sourced intake.
/// </summary>
public class OpcCommissionHandler : INotificationHandler<EnrollmentCreatedDomainEvent>
{
    private readonly ICommissionService _commissionService;

    public OpcCommissionHandler(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    public async Task Handle(EnrollmentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _commissionService.ProcessOpcCommissionAsync(
            enrollmentId: notification.EnrollmentId,
            studentId: notification.StudentId);
    }
}
