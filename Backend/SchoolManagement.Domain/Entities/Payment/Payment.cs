using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities.Payment;

public class Payment : BaseEntity
{
    public decimal Amount { get; set; }         
    public decimal? TransferFees { get; set; }   
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime PaidAt { get; set; }
    public Guid BranchId { get; set; }
    public Guid ReceivedByStaffId { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string MethodDetailsJson { get; set; } = "{}"; // stores method-specific fields as JSON

    public virtual IEnumerable<PaymentAllocation> Allocations { get; private set; } = null!;

    private Payment() { }

    public static Payment Create(Guid enrollmentId, decimal feeAmount, DateTime periodStart, DateTime periodEnd, decimal amountPaid = 0, DateTime? paidAt = null, PaymentStatus status = PaymentStatus.Pending)
    {
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID must not be empty.");
        if (feeAmount < 0)
            throw new DomainException("Fee amount cannot be negative.");
        if (amountPaid < 0)
            throw new DomainException("Amount paid cannot be negative.");

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

    public void UpdateStatus(PaymentStatus status)
    {
        Status = status;
    }
}