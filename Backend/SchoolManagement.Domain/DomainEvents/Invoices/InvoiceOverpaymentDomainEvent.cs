using MediatR;

namespace SchoolManagement.Domain.DomainEvents.Invoices;

public class InvoiceOverpaymentDomainEvent : INotification
{
    public Guid InvoiceId { get; }
    public Guid EnrollmentId { get; }
    public decimal AppliedAmount { get; }
    public decimal OverpaymentAmount { get; }
    public DateTime OccurredAt { get; }

    public InvoiceOverpaymentDomainEvent(
        Guid invoiceId,
        Guid enrollmentId,
        decimal appliedAmount,
        decimal overpaymentAmount)
    {
        InvoiceId = invoiceId;
        EnrollmentId = enrollmentId;
        AppliedAmount = appliedAmount;
        OverpaymentAmount = overpaymentAmount;
        OccurredAt = DateTime.UtcNow;
    }
}
