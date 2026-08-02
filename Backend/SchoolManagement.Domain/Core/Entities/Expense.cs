using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Domain.Core.Entities;

/// <summary>
/// Records a cash outflow from the school — simple CRUD, no approval pipeline.
/// Cash already left the drawer; this is the historical record for financial reporting.
/// </summary>
public class Expense : AggregateRoot
{
    public ExpenseType Category { get; private set; }
    public string PayeeName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string? Reference { get; private set; }
    public Guid ProcessedByStaffId { get; private set; }
    public Guid BranchId { get; private set; }
    public string CurrencyCode { get; private set; } = "MAD";

    public virtual Branch Branch { get; private set; } = null!;

    private Expense() { }

    public static Expense Create(
        ExpenseType category,
        string payeeName,
        decimal amount,
        DateTime expenseDate,
        PaymentMethod paymentMethod,
        Guid branchId,
        Guid processedByStaffId,
        string? description = null,
        string? reference = null)
    {
        if (string.IsNullOrWhiteSpace(payeeName))
            throw new DomainException("Payee name cannot be empty.");
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        if (processedByStaffId == Guid.Empty)
            throw new DomainException("Staff ID must not be empty.");

        return new Expense
        {
            Category = category,
            PayeeName = payeeName,
            Amount = amount,
            ExpenseDate = expenseDate,
            PaymentMethod = paymentMethod,
            BranchId = branchId,
            ProcessedByStaffId = processedByStaffId,
            Description = description,
            Reference = reference
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

    public void UpdateExpenseDate(DateTime expenseDate)
    {
        ExpenseDate = expenseDate;
    }

    public void UpdatePaymentMethod(PaymentMethod paymentMethod)
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

    public void UpdateProcessedByStaffId(Guid processedByStaffId)
    {
        if (processedByStaffId == Guid.Empty)
            throw new DomainException("Staff ID must not be empty.");
        ProcessedByStaffId = processedByStaffId;
    }
}
