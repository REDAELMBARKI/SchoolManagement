using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid EnrollmentId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal? TransferFees { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime PaidAt { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ReceivedByStaffId { get; private set; }
    public string? ExternalReferenceCode { get; private set; }
    public string MethodDetailsJson { get; private set; } = "{}";

    public virtual Enrollment Enrollment { get; private set; } = null!;

    private Payment() { }

    public static Payment Create(
        Guid enrollmentId,
        decimal amount,
        PaymentStatus status,
        DateTime paidAt,
        Guid branchId,
        Guid receivedByStaffId,
        decimal? transferFees = null,
        PaymentMethod method = PaymentMethod.Cash,
        string? externalReferenceCode = null,
        string methodDetailsJson = "{}")
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
            Amount = amount,
            TransferFees = transferFees,
            Method = method,
            Status = status,
            PaidAt = paidAt,
            BranchId = branchId,
            ReceivedByStaffId = receivedByStaffId,
            ExternalReferenceCode = externalReferenceCode,
            MethodDetailsJson = methodDetailsJson
        };
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
}
