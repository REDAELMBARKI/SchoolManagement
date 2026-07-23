using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Charge : AggregateRoot
{
    public Guid StudentId { get; private set; }
    public ChargeType ChargeType { get; private set; }
    public string? Description { get; private set; }
    public decimal Amount { get; private set; }
    public decimal AmountPaid { get; private set; }
    public ChargeStatus Status { get; private set; }
    public DateTime IssuedDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public Guid? SourceId { get; private set; }
    public Guid BranchId { get; private set; }

    // Navigation properties
    public virtual Student Student { get; private set; } = null!;
    public virtual Branch Branch { get; private set; } = null!;
    public virtual ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    private Charge() { }

    public static Charge Create(
        Guid studentId,
        ChargeType type,
        decimal amount,
        DateTime issuedDate,
        DateTime dueDate,
        Guid branchId ,
        string? description = null,
        decimal amountPaid = 0,
        Guid? sourceId = null
         )
    {
        if (studentId == Guid.Empty)
            throw new DomainException("Student ID must not be empty.");
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");
        if (amountPaid < 0)
            throw new DomainException("Amount paid cannot be negative.");

        var charge = new Charge
        {
            StudentId = studentId,
            ChargeType = type,
            Amount = amount,
            AmountPaid = amountPaid,
            IssuedDate = issuedDate,
            DueDate = dueDate,
            Description = description,
            SourceId = sourceId,
            BranchId = branchId
        };

        charge.UpdateStatus();
        return charge;
    }

    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");
        Amount = amount;
        UpdateStatus();
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdateDueDate(DateTime dueDate)
    {
        DueDate = dueDate;
        UpdateStatus();
    }

    public void AddPayment(decimal paymentAmount)
    {
        if (paymentAmount <= 0)
            throw new DomainException("Payment amount must be greater than zero.");
        AmountPaid += paymentAmount;
        UpdateStatus();
    }

    public void Cancel()
    {
        Status = ChargeStatus.Cancelled;
    }


    private void UpdateStatus()
    {
        // If already Cancelled or Waived, don't change
        if (Status == ChargeStatus.Cancelled )
            return;

        if (AmountPaid >= Amount)
        {
            Status = ChargeStatus.Paid;
        }
        else if (AmountPaid > 0)
        {
            Status = ChargeStatus.PartiallyPaid;
        }

    }
}