using MediatR;

namespace SchoolManagement.Domain.DomainEvents.Invoices;

public class InvoiceCancelledDomainEvent : INotification
{
    public Guid InvoiceId { get; }
    public Guid EnrollmentId { get; }
    public string Reason { get; }
    public DateTime CancelledAt { get; }

    public InvoiceCancelledDomainEvent(Guid invoiceId, Guid enrollmentId, string reason)
    {
        InvoiceId = invoiceId;
        EnrollmentId = enrollmentId;
        Reason = reason;
        CancelledAt = DateTime.UtcNow;
    }
}
