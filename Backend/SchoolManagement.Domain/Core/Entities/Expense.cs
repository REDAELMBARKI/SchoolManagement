using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Domain.Core.Entities;

public class Expense : AggregateRoot
{
    public ExpenseType Category { get; private set; }
    public string PayeeName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Amount { get; private set; }
    public ExpenseStatus Status { get; private set; }
    public Guid? RequestedBy { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime RequestedDate { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public string? Reference { get; private set; }
    public Guid BranchId { get; private set; }

    public virtual Branch Branch { get; private set; } = null!;

    private Expense() { }

    public static Expense Create(
        ExpenseType category,
        string payeeName,
        decimal amount,
        DateTime requestedDate,
        Guid branchId,
        string? description = null,
        Guid? requestedBy = null)
    {
        if (string.IsNullOrWhiteSpace(payeeName))
            throw new DomainException("Payee name cannot be empty.");
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");

        return new Expense
        {
            Category = category,
            PayeeName = payeeName,
            Amount = amount,
            RequestedDate = requestedDate,
            Description = description,
            RequestedBy = requestedBy,
            Status = ExpenseStatus.Pending,
            BranchId = branchId
        };
    }

    public void UpdateCategory(ExpenseType category)
    {
        Category = category;
    }

    public void UpdatePayeeName(string payeeName)
    {
        if (string.IsNullOrWhiteSpace(payeeName))
            throw new DomainException("Payee name cannot be empty.");
        PayeeName = payeeName;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");
        Amount = amount;
    }

    public void UpdateStatus(ExpenseStatus status)
    {
        Status = status;
    }

    public void UpdateRequestedBy(Guid? requestedBy)
    {
        RequestedBy = requestedBy;
    }

    public void UpdateApprovedBy(Guid? approvedBy)
    {
        ApprovedBy = approvedBy;
    }

    public void UpdateRequestedDate(DateTime requestedDate)
    {
        RequestedDate = requestedDate;
    }

    public void UpdatePaidDate(DateTime? paidDate)
    {
        PaidDate = paidDate;
    }

    public void UpdatePaymentMethod(PaymentMethod? paymentMethod)
    {
        PaymentMethod = paymentMethod;
    }

    public void UpdateReference(string? reference)
    {
        Reference = reference;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        BranchId = branchId;
    }
}
