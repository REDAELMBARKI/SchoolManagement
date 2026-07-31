using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

public class InvoiceWaivedDomainEvent : INotification
{
    public Guid InvoiceId { get; }
    public Guid EnrollmentId { get; }
    public decimal WaivedAmount { get; }
    public string Reason { get; }
    public DateTime WaivedAt { get; }

    public InvoiceWaivedDomainEvent(Guid invoiceId, Guid enrollmentId, decimal waivedAmount, string reason)
    {
        InvoiceId = invoiceId;
        EnrollmentId = enrollmentId;
        WaivedAmount = waivedAmount;
        Reason = reason;
        WaivedAt = DateTime.UtcNow;
    }
}
