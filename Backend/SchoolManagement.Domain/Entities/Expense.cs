using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

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

    private Expense() { }

    public static Expense Create(
        ExpenseType category,
        string payeeName,
        decimal amount,
        DateTime requestedDate,
        string? description = null,
        Guid? requestedBy = null)
    {
        if (string.IsNullOrWhiteSpace(payeeName))
            throw new DomainException("Payee name cannot be empty.");
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");

        return new Expense
        {
            Category = category,
            PayeeName = payeeName,
            Amount = amount,
            RequestedDate = requestedDate,
            Description = description,
            RequestedBy = requestedBy,
            Status = ExpenseStatus.Pending
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

    public void Approve(Guid approvedBy)
    {
        if (approvedBy == Guid.Empty)
            throw new DomainException("Approved by ID must not be empty.");
        Status = ExpenseStatus.Approved;
        ApprovedBy = approvedBy;
    }

    public void MarkAsPaid(DateTime paidDate, PaymentMethod paymentMethod, string? reference = null)
    {
        if (paymentMethod == 0)
            throw new DomainException("Payment method must be selected.");
        Status = ExpenseStatus.Paid;
        PaidDate = paidDate;
        PaymentMethod = paymentMethod;
        Reference = reference;
    }

    public void Reject()
    {
        Status = ExpenseStatus.Rejected;
    }
}