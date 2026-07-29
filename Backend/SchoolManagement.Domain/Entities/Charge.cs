using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Charge : AggregateRoot
{
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; }

    // Navigation property
    public virtual Invoice Invoice { get; private set; } = null!;

    private Charge() { }

    public static Charge Create(
        Guid invoiceId,
        decimal amount,
        DateTime dueDate)
    {
        if (invoiceId == Guid.Empty)
            throw new DomainException("Invoice ID must not be empty.");
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");

        return new Charge
        {
            InvoiceId = invoiceId,
            Amount = amount,
            DueDate = dueDate
        };
    }

    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");
        Amount = amount;
    }

    public void UpdateDueDate(DateTime dueDate)
    {
        DueDate = dueDate;
    }
}
