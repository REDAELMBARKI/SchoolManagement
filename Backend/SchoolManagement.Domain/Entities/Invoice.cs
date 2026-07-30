using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.DomainEvents.Invoices;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

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

    // Navigation & Collection of Charges
    private readonly List<Charge> _charges = new();
    public virtual IReadOnlyCollection<Charge> Charges => _charges.AsReadOnly();

    public virtual ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    // Navigation property
    public virtual Enrollment Enrollment { get; private set; } = null!;
    public virtual Branch Branch { get; private set; } = null!;

    // TotalAmount sum of its active charges minus waived amount
    public decimal TotalAmount => _charges.Where(c => c.Status == ChargeStatus.Active).Sum(c => c.Amount - c.WaivedAmount);

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

        _charges.Add(charge);
        RecalculateStatus();
    }

    public void AddPayment(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero.");

        PaidAmount += amount;
        RecalculateStatus();
    }

    public void RecordCreditApplied(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Credit applied amount cannot be negative.");
        CreditAppliedAmount += amount;
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

        decimal remainingToWaive = waivedAmount;
        var activeCharges = _charges.Where(c => c.Status == ChargeStatus.Active).ToList();

        foreach (var charge in activeCharges)
        {
            if (remainingToWaive <= 0) break;

            var chargeRemaining = charge.Amount - charge.WaivedAmount - charge.PaidAmount;
            if (chargeRemaining <= 0) continue;

            var amountToWaive = Math.Min(remainingToWaive, chargeRemaining);
            charge.Waive(reason, amountToWaive);
            remainingToWaive -= amountToWaive;
        }

        RecalculateStatus();

        if (_charges.Count > 0 && _charges.All(c => c.Status == ChargeStatus.Waived))
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
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required.");

        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Invoice is already cancelled.");

        if (Status != InvoiceStatus.Pending && Status != InvoiceStatus.PartiallyPaid)
            throw new DomainException("Only pending or partially paid invoices can be cancelled.");

        foreach (var charge in _charges.Where(c => c.Status == ChargeStatus.Active))
        {
            charge.Cancel();
        }

        Status = InvoiceStatus.Cancelled;
        AddDomainEvent(new InvoiceCancelledDomainEvent(Id, EnrollmentId, reason));
    }

    public void RecalculateStatus()
    {
        if (Status == InvoiceStatus.Cancelled)
            return;

        if (_charges.Count > 0 && _charges.All(c => c.Status == ChargeStatus.Waived))
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
    }
}
