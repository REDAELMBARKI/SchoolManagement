using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.DomainEvents;
using SchoolManagement.Domain.Core.Enums;
using SchoolManagement.Domain.Core.Results;

namespace SchoolManagement.Domain.Core.Entities;

public class Invoice : AggregateRoot
{
    public Guid EnrollmentId { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public DateTime DueDate { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal CreditAppliedAmount { get; private set; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Pending;
    public Guid BranchId { get; private set; }

    // Navigation
    public virtual Charge? Charge { get; private set; }

    public virtual ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    // Navigation property
    public virtual Enrollment Enrollment { get; private set; } = null!;
    public virtual Branch Branch { get; private set; } = null!;

    // TotalAmount is the net liability for the invoice's single charge, if present.
    public decimal TotalAmount =>
        Charge == null || Charge.Status == ChargeStatus.Cancelled
            ? 0
            : Charge.Amount - Charge.WaivedAmount;

    private Invoice() { }

    public static Invoice Create(
        Guid enrollmentId,
        DateTime periodStart,
        DateTime periodEnd,
        DateTime dueDate,
        Guid branchId,
        decimal paidAmount = 0)
    {
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID must not be empty.");
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        if (periodEnd < periodStart)
            throw new DomainException("Period end date cannot be earlier than period start date.");

        var invoice = new Invoice
        {
            EnrollmentId = enrollmentId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            DueDate = dueDate,
            BranchId = branchId,
            PaidAmount = paidAmount,
            Status = InvoiceStatus.Pending
        };

        invoice.RecalculateStatus();
        return invoice;
    }

    public void AddCharge(Charge charge)
    {
        if (charge == null)
            throw new DomainException("Charge cannot be null.");
        if (Charge != null)
            throw new DomainException("Only one charge is allowed per invoice.");

        Charge = charge;
        RecalculateStatus();
    }

    public InvoicePaymentResult AddPayment(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero.");
        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Cannot add payment to a cancelled invoice.");

        var remainingBalance = Math.Max(0, TotalAmount - PaidAmount);
        var appliedAmount = Math.Min(amount, remainingBalance);
        if (Charge != null &&
            (Charge.Status == ChargeStatus.Active || Charge.Status == ChargeStatus.PartiallyPaid))
        {
            Charge.AddPayment(appliedAmount);
        }

        PaidAmount += appliedAmount;
        RecalculateStatus();

        var overpaymentAmount = amount - appliedAmount;
        if (overpaymentAmount > 0)
        {
            AddDomainEvent(new InvoiceOverpaymentDomainEvent(
                Id,
                EnrollmentId,
                appliedAmount,
                overpaymentAmount));
        }

        return new InvoicePaymentResult(appliedAmount, overpaymentAmount);
    }

    public void RecordCreditApplied(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Credit applied amount cannot be negative.");
        CreditAppliedAmount += amount;
    }

    /// <summary>
    /// Reduces PaidAmount when a refund is issued against a payment on this invoice.
    /// Recalculates status after deduction.
    /// </summary>
    public void DeductRefund(decimal refundAmount)
    {
        if (refundAmount <= 0)
            throw new DomainException("Refund amount must be greater than zero.");
        if (refundAmount > PaidAmount)
            throw new DomainException("Refund amount cannot exceed the invoice paid amount.");

        PaidAmount -= refundAmount;

        // Reverse the charge payment tracking if charge exists
        if (Charge != null &&
            (Charge.Status == ChargeStatus.Paid || Charge.Status == ChargeStatus.PartiallyPaid))
        {
            Charge.ReversePayment(refundAmount);
        }

        RecalculateStatus();
    }

    public void WaiveInvoice(decimal waivedAmount, string reason)
    {
        if (waivedAmount <= 0)
            throw new DomainException("Waived amount must be greater than zero.");

        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Cannot waive a cancelled invoice.");

        if (Status == InvoiceStatus.Waived)
            throw new DomainException("Invoice is already waived.");

        var remainingBalance = TotalAmount - PaidAmount;
        if (waivedAmount > remainingBalance)
            throw new DomainException("Waived amount cannot exceed the remaining balance.");

        if (Charge == null)
            throw new DomainException("Cannot waive an invoice without a charge.");

        if (Charge.Status == ChargeStatus.Cancelled || Charge.Status == ChargeStatus.Waived)
            throw new DomainException("Only an active invoice charge can be waived.");

        Charge.Waive(reason, waivedAmount);

        RecalculateStatus();

        if (Charge?.Status == ChargeStatus.Waived)
        {
            Status = InvoiceStatus.Waived;
        }
        else if (waivedAmount >= remainingBalance)
        {
            Status = InvoiceStatus.Waived;
        }

        AddDomainEvent(new InvoiceWaivedDomainEvent(Id, EnrollmentId, waivedAmount, reason));
    }

    public void CancelInvoice(string reason)
    {        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required.");

        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Invoice is already cancelled.");

        if (Status != InvoiceStatus.Pending && Status != InvoiceStatus.PartiallyPaid)
            throw new DomainException("Only pending or partially paid invoices can be cancelled.");

        if (Charge != null &&
            Charge.Status != ChargeStatus.Cancelled &&
            Charge.Status != ChargeStatus.Waived)
        {
            Charge.Cancel();
        }

        Status = InvoiceStatus.Cancelled;
        AddDomainEvent(new InvoiceCancelledDomainEvent(Id, EnrollmentId, reason));
    }

    public void RecalculateStatus()
    {        
        if (Status == InvoiceStatus.Cancelled)
            return;

        var oldStatus = Status;

        if (Charge?.Status == ChargeStatus.Waived)
        {
            Status = InvoiceStatus.Waived;
            return;
        }

        if (PaidAmount >= TotalAmount && TotalAmount > 0)
        {
            Status = InvoiceStatus.Paid;
        }
        else if (PaidAmount > 0)
        {
            Status = InvoiceStatus.PartiallyPaid;
        }
        else if (DateTime.UtcNow > DueDate && PaidAmount < TotalAmount)
        {
            Status = InvoiceStatus.PastDue;
        }
        else
        {
            Status = InvoiceStatus.Pending;
        }

        if (Status == InvoiceStatus.PastDue && oldStatus != InvoiceStatus.PastDue)
        {
            var amountDue = TotalAmount - PaidAmount;
            AddDomainEvent(new InvoiceOverdueDomainEvent(Id, EnrollmentId, DateTime.UtcNow, amountDue));
        }
    }
}
