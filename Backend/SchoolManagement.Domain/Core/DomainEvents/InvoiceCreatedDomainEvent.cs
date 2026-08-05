using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

/// <summary>
/// Minimal event - handler will fetch invoice details including TotalAmount after DB save
/// </summary>
public class InvoiceCreatedDomainEvent : INotification
{
    public Guid InvoiceId { get; }
    public Guid EnrollmentId { get; }
    public Guid BranchId { get; }
    public DateTime DueDate { get; }

    public InvoiceCreatedDomainEvent(
        Guid invoiceId,
        Guid enrollmentId,
        Guid branchId,
        DateTime dueDate)
    {
        InvoiceId = invoiceId;
        EnrollmentId = enrollmentId;
        BranchId = branchId;
        DueDate = dueDate;
    }
}
