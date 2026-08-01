using MediatR;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Core.DomainEvents;

namespace SchoolManagement.Application.Core.EventsHandlers;

/// <summary>
/// Listens for EnrollmentDroppedDomainEvent and automatically blocks
/// the linked OPC commission if the salary lockout hasn't passed yet.
/// </summary>
public class EnrollmentDroppedCommissionHandler : INotificationHandler<EnrollmentDroppedDomainEvent>
{
    private readonly ICommissionService _commissionService;

    public EnrollmentDroppedCommissionHandler(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    public async Task Handle(EnrollmentDroppedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _commissionService.BlockOpcCommissionByEnrollmentAsync(
            enrollmentId: notification.EnrollmentId,
            reason: $"Enrollment dropped. Reason: {notification.Reason}");
    }
}
