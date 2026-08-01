using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Domain.Core.Entities;

public class Payment : AggregateRoot
{
    public Guid EnrollmentId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal? TransferFees { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime PaidAt { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ReceivedByStaffId { get; private set; }
    public string? ExternalReferenceCode { get; private set; }
    public string MethodDetailsJson { get; private set; } = "{}";
    public string CurrencyCode { get; private set; } = "MAD";

    public virtual Enrollment Enrollment { get; private set; } = null!;
    public virtual Invoice? Invoice { get; private set; }
    public virtual ICollection<Refund> Refunds { get; private set; } = new List<Refund>();

    private Payment() { }

    public static Payment Create(
        Guid enrollmentId,
        decimal amount,
        PaymentStatus status,
        DateTime paidAt,
        Guid branchId,
        Guid receivedByStaffId,
        Guid? invoiceId = null,
        decimal? transferFees = null,
        PaymentMethod method = PaymentMethod.Cash,
        string? externalReferenceCode = null,
        string methodDetailsJson = "{}"
     )
    {
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID must not be empty.");
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");
        if (transferFees.HasValue && transferFees.Value < 0)
            throw new DomainException("Transfer fees cannot be negative.");
        if (string.IsNullOrWhiteSpace(methodDetailsJson))
            throw new DomainException("Method details JSON cannot be empty.");

        return new Payment
        {
            EnrollmentId = enrollmentId,
            InvoiceId = invoiceId,
            Amount = amount,
            TransferFees = transferFees,
            Method = method,
            Status = status,
            PaidAt = paidAt,
            BranchId = branchId,
            ReceivedByStaffId = receivedByStaffId,
            ExternalReferenceCode = externalReferenceCode,
            MethodDetailsJson = methodDetailsJson,
        };
    }

    public void UpdateInvoiceId(Guid? invoiceId)
    {
        InvoiceId = invoiceId;
    }

    public void UpdateEnrollmentId(Guid enrollmentId)
    {
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID must not be empty.");
        EnrollmentId = enrollmentId;
    }

    public void UpdateAmount(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");
        Amount = amount;
    }

    public void UpdateTransferFees(decimal? transferFees)
    {
        if (transferFees.HasValue && transferFees.Value < 0)
            throw new DomainException("Transfer fees cannot be negative.");
        TransferFees = transferFees;
    }

    public void UpdateMethod(PaymentMethod method)
    {
        Method = method;
    }

    public void UpdateStatus(PaymentStatus status)
    {
        Status = status;
    }

    public void UpdatePaidAt(DateTime paidAt)
    {
        PaidAt = paidAt;
    }

    public void UpdateBranchId(Guid branchId)
    {
        BranchId = branchId;
    }

    public void UpdateReceivedByStaffId(Guid receivedByStaffId)
    {
        ReceivedByStaffId = receivedByStaffId;
    }

    public void UpdateExternalReferenceCode(string? externalReferenceCode)
    {
        ExternalReferenceCode = externalReferenceCode;
    }

    public void UpdateMethodDetailsJson(string methodDetailsJson)
    {
        if (string.IsNullOrWhiteSpace(methodDetailsJson))
            throw new DomainException("Method details JSON cannot be empty.");
        MethodDetailsJson = methodDetailsJson;
    }

    /// <summary>Total amount already refunded across all refund records.</summary>
    public decimal GetTotalRefunded() => Refunds.Sum(r => r.Amount);

    /// <summary>Amount still available to refund.</summary>
    public decimal GetRefundableAmount() => Amount - GetTotalRefunded();

    /// <summary>
    /// Marks the payment as Refunded once it has been fully refunded.
    /// Called after a refund record is saved.
    /// </summary>
    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
    }
}
