using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities.EnrollmentAggregate;

public class Payment : BaseEntity
{
    public DateTime? PaidAt { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public decimal AmountPaid { get; private set; }
    public decimal FeeAmount { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public string Status { get; private set; } = "Pending";

    public virtual Enrollment Enrollment { get; private set; } = null!;

    private Payment() { }

    public static Payment Create(Guid enrollmentId, decimal feeAmount, DateTime periodStart, DateTime periodEnd, decimal amountPaid = 0, DateTime? paidAt = null, string status = "Pending")
    {
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID must not be empty.");
        if (feeAmount < 0)
            throw new DomainException("Fee amount cannot be negative.");
        if (amountPaid < 0)
            throw new DomainException("Amount paid cannot be negative.");
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty.");

        return new Payment
        {
            EnrollmentId = enrollmentId,
            FeeAmount = feeAmount,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            AmountPaid = amountPaid,
            PaidAt = paidAt,
            Status = status
        };
    }

    public void UpdatePaidAt(DateTime? paidAt)
    {
        PaidAt = paidAt;
    }

    public void UpdateEnrollmentId(Guid enrollmentId)
    {
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID must not be empty.");
        EnrollmentId = enrollmentId;
    }

    public void UpdateAmountPaid(decimal amountPaid)
    {
        if (amountPaid < 0)
            throw new DomainException("Amount paid cannot be negative.");
        AmountPaid = amountPaid;
    }

    public void UpdateFeeAmount(decimal feeAmount)
    {
        if (feeAmount < 0)
            throw new DomainException("Fee amount cannot be negative.");
        FeeAmount = feeAmount;
    }

    public void UpdatePeriodStart(DateTime periodStart)
    {
        PeriodStart = periodStart;
    }

    public void UpdatePeriodEnd(DateTime periodEnd)
    {
        PeriodEnd = periodEnd;
    }

    public void UpdateStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty.");
        Status = status;
    }
}