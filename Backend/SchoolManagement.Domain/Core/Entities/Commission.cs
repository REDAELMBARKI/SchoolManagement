using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Domain.Core.Entities;

public class Commission : AggregateRoot
{
    public Guid EarnerId { get; private set; }
    public EarnerType EarnerType { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly PeriodMonth { get; private set; }
    public CommissionStatus Status { get; private set; } = CommissionStatus.Approved;

    // OPC only — the enrollment that triggered this commission
    public Guid? SourceEnrollmentId { get; private set; }

    // Commercial Agent only
    public int? SalesCountAtCalculation { get; private set; }
    public int? AppliedTierMin { get; private set; }
    public int? AppliedTierMax { get; private set; }

    // Set when the commission is blocked — for audit visibility
    public string? BlockReason { get; private set; }

    private Commission() { }

    /// <summary>
    /// Creates an OPC commission. Starts as Approved immediately —
    /// enrollment is active so the commission is earned right away.
    /// </summary>
    public static Commission CreateForOpc(Guid opcId, decimal amount, DateOnly periodMonth, Guid enrollmentId)
    {
        if (opcId == Guid.Empty)
            throw new DomainException("OPC ID must not be empty.");
        if (amount <= 0)
            throw new DomainException("Commission amount must be greater than zero.");
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID must not be empty.");

        return new Commission
        {
            EarnerId = opcId,
            EarnerType = EarnerType.Opc,
            Amount = amount,
            PeriodMonth = periodMonth,
            Status = CommissionStatus.Approved,
            SourceEnrollmentId = enrollmentId
        };
    }

    /// <summary>
    /// Creates a Commercial Agent monthly tiered commission.
    /// Starts as Approved — agent earned it based on their monthly sales count.
    /// </summary>
    public static Commission CreateForAgent(
        Guid agentId,
        decimal amount,
        DateOnly periodMonth,
        int salesCount,
        int tierMin,
        int? tierMax)
    {
        if (agentId == Guid.Empty)
            throw new DomainException("Agent ID must not be empty.");
        if (amount <= 0)
            throw new DomainException("Commission amount must be greater than zero.");
        if (salesCount <= 0)
            throw new DomainException("Sales count must be greater than zero.");

        return new Commission
        {
            EarnerId = agentId,
            EarnerType = EarnerType.CommercialAgent,
            Amount = amount,
            PeriodMonth = periodMonth,
            Status = CommissionStatus.Approved,
            SalesCountAtCalculation = salesCount,
            AppliedTierMin = tierMin,
            AppliedTierMax = tierMax
        };
    }

    /// <summary>
    /// Blocks a commission — called when enrollment is dropped or cancelled,
    /// or manually by a manager. Only allowed before salary lockout (Paid).
    /// </summary>
    public void Block(string reason)
    {
        if (Status == CommissionStatus.Paid)
            throw new DomainException("Cannot block a commission that has already been paid. Salary cutoff has passed.");

        if (Status == CommissionStatus.Blocked)
            throw new DomainException("Commission is already blocked.");

        Status = CommissionStatus.Blocked;
        BlockReason = reason;
    }


    public void Approve()
    {
        if (Status == CommissionStatus.Paid)
            throw new DomainException("Cannot approve a commission that has already been paid. Salary cutoff has passed.");
        if (Status == CommissionStatus.Approved)
            throw new DomainException("Commission is already approved.");
        Status = CommissionStatus.Approved;
        BlockReason = null; // Clear block reason if it was previously blocked
    }

    /// <summary>
    /// Called by the salary day lockout job. Marks as Paid.
    /// Only Approved commissions become Paid — Blocked ones stay Blocked.
    /// </summary>
    public void MarkAsPaid()
    {
        if (Status != CommissionStatus.Approved)
            throw new DomainException("Only approved commissions can be marked as paid.");

        Status = CommissionStatus.Paid;
    }
}
