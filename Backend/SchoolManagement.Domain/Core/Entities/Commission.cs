using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Domain.Core.Entities;

/// <summary>
/// Represents a commission earned by an OPC or Commercial Agent.
/// This is an IMMUTABLE financial record - once created, the Amount never changes.
/// 
/// Design principles:
/// - Amount is ALWAYS stored directly on the Commission (snapshot pattern)
/// - For OPC: Amount comes from flat rate configuration
/// - For Agent: Amount is copied from the CommissionTier at calculation time
/// - If tier amounts change later, historical commissions retain their original values
/// - CommissionTierId is optional FK for traceability (required for Agent, null for OPC)
/// </summary>
public class Commission : AggregateRoot
{
    public Guid EarnerId { get; private set; }
    public EarnerType EarnerType { get; private set; }
    
    /// <summary>
    /// The commission amount paid. This is ALWAYS populated for both OPC and Agent commissions.
    /// This is a historical snapshot - it never changes even if tier amounts are updated later.
    /// - For OPC: Flat rate amount (e.g., $50 per enrollment)
    /// - For Agent: Amount copied from the tier that was active at calculation time
    /// </summary>
    public decimal Amount { get; private set; }
    
    public DateOnly PeriodMonth { get; private set; }
    public CommissionStatus Status { get; private set; } = CommissionStatus.Approved;
    
    /// <summary>
    /// Branch ID for multi-tenant filtering and reporting.
    /// </summary>
    public Guid BranchId { get; private set; }

    /// <summary>
    /// Optional FK to CommissionTier for traceability and reporting.
    /// - For OPC commissions: NULL (no tier system, flat rate)
    /// - For Agent commissions: REQUIRED (references the tier used for calculation)
    /// This allows queries like "show all commissions calculated with Tier X" while
    /// preserving the historical Amount snapshot.
    /// </summary>
    public Guid? CommissionTierId { get; private set; }

    /// <summary>
    /// Navigation property to the CommissionTier (for EF Core).
    /// NULL for OPC commissions, populated for Agent commissions.
    /// </summary>
    public CommissionTier? CommissionTier { get; private set; }

    /// <summary>
    /// OPC only — the enrollment that triggered this commission.
    /// Used for traceability and auto-blocking if enrollment is dropped.
    /// </summary>
    public Guid? SourceEnrollmentId { get; private set; }

    /// <summary>
    /// Commercial Agent only — number of sales the agent had when this commission was calculated.
    /// Stored as a snapshot for audit and reporting purposes.
    /// This allows you to see "Agent had 15 sales in March 2024" without recalculating.
    /// </summary>
    public int? SalesCountAtCalculation { get; private set; }

    /// <summary>
    /// Set when the commission is blocked — for audit visibility.
    /// Explains why this commission was blocked (e.g., "Enrollment dropped", "Manual block by manager").
    /// </summary>
    public string? BlockReason { get; private set; }

    private Commission() { }

    /// <summary>
    /// Creates an OPC commission. Starts as Approved immediately —
    /// enrollment is active so the commission is earned right away.
    /// CommissionTierId is null for OPC commissions (no tier system).
    /// </summary>
    public static Commission CreateForOpc(Guid opcId, decimal amount, DateOnly periodMonth, Guid enrollmentId, Guid branchId)
    {
        if (opcId == Guid.Empty)
            throw new DomainException("OPC ID must not be empty.");
        if (amount <= 0)
            throw new DomainException("Commission amount must be greater than zero.");
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID must not be empty.");
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");

        return new Commission
        {
            EarnerId = opcId,
            EarnerType = EarnerType.Opc,
            Amount = amount,
            PeriodMonth = periodMonth,
            Status = CommissionStatus.Approved,
            SourceEnrollmentId = enrollmentId,
            CommissionTierId = null, // OPC commissions don't use tiers
            BranchId = branchId
        };
    }

    /// <summary>
    /// Creates a Commercial Agent monthly tiered commission.
    /// Starts as Approved — agent earned it based on their monthly sales count.
    /// CommissionTierId is REQUIRED for agent commissions.
    /// </summary>
    public static Commission CreateForAgent(
        Guid agentId,
        decimal amount,
        DateOnly periodMonth,
        int salesCount,
        Guid commissionTierId,
        Guid branchId)
    {
        if (agentId == Guid.Empty)
            throw new DomainException("Agent ID must not be empty.");
        if (amount <= 0)
            throw new DomainException("Commission amount must be greater than zero.");
        if (salesCount <= 0)
            throw new DomainException("Sales count must be greater than zero.");
        if (commissionTierId == Guid.Empty)
            throw new DomainException("CommissionTierId is required for Commercial Agent commissions.");
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");

        return new Commission
        {
            EarnerId = agentId,
            EarnerType = EarnerType.CommercialAgent,
            Amount = amount,
            PeriodMonth = periodMonth,
            Status = CommissionStatus.Approved,
            SalesCountAtCalculation = salesCount,
            CommissionTierId = commissionTierId,
            BranchId = branchId
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
