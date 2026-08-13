using MediatR;

namespace SchoolManagement.Domain.Common.Events;

/// <summary>
/// Event raised when an invoice is generated and needs to be sent to student
/// </summary>
public class InvoiceGeneratedEvent : INotification
{
    public string Email { get; }
    public string StudentName { get; }
    public string InvoiceNumber { get; }
    public string Description { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public DateTime InvoiceDate { get; }
    public DateTime DueDate { get; }
    public string? PaymentUrl { get; }

    public InvoiceGeneratedEvent(
        string email,
        string studentName,
        string invoiceNumber,
        string description,
        decimal amount,
        string currency = "MAD",
        DateTime? invoiceDate = null,
        DateTime? dueDate = null,
        string? paymentUrl = null)
    {
        Email = email;
        StudentName = studentName;
        InvoiceNumber = invoiceNumber;
        Description = description;
        Amount = amount;
        Currency = currency;
        InvoiceDate = invoiceDate ?? DateTime.UtcNow;
        DueDate = dueDate ?? DateTime.UtcNow.AddDays(30);
        PaymentUrl = paymentUrl;
    }
}
