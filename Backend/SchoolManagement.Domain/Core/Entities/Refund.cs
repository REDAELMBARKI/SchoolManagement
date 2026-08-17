using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Domain.Core.Entities;

public class Refund : AggregateRoot
{
    public Guid PaymentId { get; private set; }
    public decimal Amount { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime RefundedAt { get; private set; }
    public Guid RefundedByStaffId { get; private set; }
    public Guid BranchId { get; private set; }

    // Navigation
    public virtual Payment Payment { get; private set; } = null!;

    protected Refund() { }

    public static Refund Create(
        Guid paymentId,
        decimal amount,
        string reason,
        Guid refundedByStaffId,
        Guid branchId,
        DateTime? refundedAt = null)
    {
        if (paymentId == Guid.Empty)
            throw new DomainException("Payment ID must not be empty.");
        if (amount <= 0)
            throw new DomainException("Refund amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Refund reason is required.");
        if (refundedByStaffId == Guid.Empty)
            throw new DomainException("Staff ID must not be empty.");
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");

        return new Refund
        {
            PaymentId = paymentId,
            Amount = amount,
            Reason = reason,
            RefundedByStaffId = refundedByStaffId,
            BranchId = branchId,
            RefundedAt = refundedAt ?? DateTime.UtcNow
        };
    }
}
