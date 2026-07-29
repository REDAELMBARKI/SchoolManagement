using SchoolManagement.Domain.Common;
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
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Pending;
    public Guid BranchId { get; private set; }

    // Navigation & Collection of Charges
    private readonly List<Charge> _charges = new();
    public virtual IReadOnlyCollection<Charge> Charges => _charges.AsReadOnly();

    public virtual ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    // Navigation property
    public virtual Enrollment Enrollment { get; private set; } = null!;
    public virtual Branch Branch { get; private set; } = null!;

    // TotalAmount sum of its active charges
    public decimal TotalAmount => _charges.Where(c => c.Status == ChargeStatus.Active).Sum(c => c.Amount);

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

    public void RecalculateStatus()
    {
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
            Status = InvoiceStatus.Overdue;
        }
        else
        {
            Status = InvoiceStatus.Pending;
        }
    }
}
