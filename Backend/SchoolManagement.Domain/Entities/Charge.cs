using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Charge : AggregateRoot
{
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal WaivedAmount { get; private set; }
    public string? WaivedReason { get; private set; }
    public DateTime DueDate { get; private set; }
    public ChargeStatus Status { get; private set; } = ChargeStatus.Active;

    // Navigation property
    public virtual Invoice Invoice { get; private set; } = null!;

    private Charge() { }

    public static Charge Create(
        Guid invoiceId,
        decimal amount,
        DateTime dueDate,
        ChargeStatus status = ChargeStatus.Active)
    {
        if (invoiceId == Guid.Empty)
            throw new DomainException("Invoice ID must not be empty.");
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");

        return new Charge
        {
            InvoiceId = invoiceId,
            Amount = amount,
            DueDate = dueDate,
            Status = status
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

    public void Waive(string? reason = null, decimal? waivedAmount = null)
    {
        if (Status == ChargeStatus.Cancelled)
            throw new DomainException("Cannot waive a cancelled charge.");

        var amountToWaive = waivedAmount ?? (Amount - PaidAmount);
        if (amountToWaive <= 0)
            throw new DomainException("Waived amount must be greater than zero.");
        if (WaivedAmount + amountToWaive > Amount)
            throw new DomainException("Total waived amount cannot exceed charge amount.");

        WaivedAmount += amountToWaive;
        WaivedReason = reason;
        Status = ChargeStatus.Waived;
    }

    public void Cancel()
    {
        Status = ChargeStatus.Cancelled;
    }

    public void Reactivate()
    {
        Status = ChargeStatus.Active;
        WaivedAmount = 0;
        WaivedReason = null;
    }
}

