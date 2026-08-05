using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

public class PaymentReceivedDomainEvent : INotification
{
    public Guid PaymentId { get; }
    public Guid InvoiceId { get; }
    public Guid EnrollmentId { get; }
    public decimal Amount { get; }
    public DateTime PaidAt { get; }

    public PaymentReceivedDomainEvent(
        Guid paymentId,
        Guid invoiceId,
        Guid enrollmentId,
        decimal amount,
        DateTime paidAt)
    {
        PaymentId = paymentId;
        InvoiceId = invoiceId;
        EnrollmentId = enrollmentId;
        Amount = amount;
        PaidAt = paidAt;
    }
}
