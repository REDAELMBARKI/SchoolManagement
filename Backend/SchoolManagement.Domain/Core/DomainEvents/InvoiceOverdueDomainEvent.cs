using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

public class InvoiceOverdueDomainEvent : INotification
{
    public Guid InvoiceId { get; }
    public Guid EnrollmentId { get; }
    public DateTime OverdueDate { get; }
    public decimal AmountDue { get; }

    public InvoiceOverdueDomainEvent(Guid invoiceId, Guid enrollmentId, DateTime overdueDate, decimal amountDue)
    {
        InvoiceId = invoiceId;
        EnrollmentId = enrollmentId;
        OverdueDate = overdueDate;
        AmountDue = amountDue;
    }
}
