using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.DomainEvents;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Domain.Core.Entities;

public class PayrollPayment : AggregateRoot
{
    public Guid EmployeeId { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal Bonus { get; private set; }
    public decimal Deductions { get; private set; }
    public decimal NetAmount { get; private set; }
    public int PayPeriodMonth { get; private set; }
    public int PayPeriodYear { get; private set; }
    public PayrollStatus Status { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public string? ReferenceCode { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ProcessedByStaffId { get; private set; }
    public string? Notes { get; private set; }


    // Navigation
    public virtual Branch Branch { get; private set; } = null!;

    private PayrollPayment() { }

    /// <summary>
    /// Creates a pending payroll payment for an employee.
    /// GrossAmount is a snapshot of the employee's salary at the time of creation.
    /// NetAmount = GrossAmount + Bonus - Deductions.
    /// </summary>
    public static PayrollPayment Create(
        Guid employeeId,
        decimal grossAmount,
        int payPeriodMonth,
        int payPeriodYear,
        Guid branchId,
        Guid processedByStaffId,
        decimal bonus = 0,
        decimal deductions = 0,
        string? notes = null)
    {
        if (employeeId == Guid.Empty)
            throw new DomainException("Employee ID must not be empty.");
        if (grossAmount <= 0)
            throw new DomainException("Gross amount must be greater than zero.");
        if (bonus < 0)
            throw new DomainException("Bonus cannot be negative.");
        if (deductions < 0)
            throw new DomainException("Deductions cannot be negative.");
        if (payPeriodMonth < 1 || payPeriodMonth > 12)
            throw new DomainException("Pay period month must be between 1 and 12.");
        if (payPeriodYear < 2000)
            throw new DomainException("Pay period year must be a valid year.");
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        if (processedByStaffId == Guid.Empty)
            throw new DomainException("Staff ID must not be empty.");

        var netAmount = grossAmount + bonus - deductions;
        if (netAmount <= 0)
            throw new DomainException("Net amount must be greater than zero after deductions.");

        return new PayrollPayment
        {
            EmployeeId = employeeId,
            GrossAmount = grossAmount,
            Bonus = bonus,
            Deductions = deductions,
            NetAmount = netAmount,
            PayPeriodMonth = payPeriodMonth,
            PayPeriodYear = payPeriodYear,
            BranchId = branchId,
            ProcessedByStaffId = processedByStaffId,
            Notes = notes,
            Status = PayrollStatus.Pending
        };
    }

    /// <summary>
    /// Marks the payroll as paid — cash has left the drawer.
    /// Only pending payments can be marked as paid.
    /// </summary>
    public void MarkAsPaid(PaymentMethod method, string? referenceCode, DateTime? paidAt = null)
    {
        if (Status != PayrollStatus.Pending)
            throw new DomainException("Only pending payroll payments can be marked as paid.");

        Status = PayrollStatus.Paid;
        PaymentMethod = method;
        ReferenceCode = referenceCode;
        PaidAt = paidAt ?? DateTime.UtcNow;

        AddDomainEvent(new PayrollPaidDomainEvent(
            Id,
            EmployeeId,
            NetAmount,
            PayPeriodMonth,
            PayPeriodYear));
    }

    /// <summary>
    /// Updates the bonus amount and recalculates net.
    /// Only allowed while the payment is still pending.
    /// </summary>
    public void UpdateBonus(decimal bonus)
    {
        if (Status != PayrollStatus.Pending)
            throw new DomainException("Cannot update bonus on a payment that has already been paid.");
        if (bonus < 0)
            throw new DomainException("Bonus cannot be negative.");

        Bonus = bonus;
        RecalculateNet();
    }

    /// <summary>
    /// Updates the deductions amount and recalculates net.
    /// Only allowed while the payment is still pending.
    /// </summary>
    public void UpdateDeductions(decimal deductions)
    {
        if (Status != PayrollStatus.Pending)
            throw new DomainException("Cannot update deductions on a payment that has already been paid.");
        if (deductions < 0)
            throw new DomainException("Deductions cannot be negative.");

        Deductions = deductions;
        RecalculateNet();
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }

    private void RecalculateNet()
    {
        NetAmount = GrossAmount + Bonus - Deductions;
        if (NetAmount <= 0)
            throw new DomainException("Net amount must be greater than zero after deductions.");
    }
}
