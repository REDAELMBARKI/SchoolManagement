using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface ICommissionRepository : IRepository<Commission>
{
    /// <summary>
    /// Counts enrollments attributable to an agent for a given calendar month,
    /// walking the chain: Enrollment → Student → Intake → CommercialAgentId.
    /// Used for commission calculation.
    /// </summary>
    Task<int> CountAgentEnrollmentsForMonthAsync(Guid agentId, int year, int month);

    /// <summary>
    /// Returns true if an OPC commission has already been recorded for this enrollment,
    /// preventing duplicate commissions if the event fires more than once.
    /// Idempotency check before write operation.
    /// </summary>
    Task<bool> OpcCommissionExistsForEnrollmentAsync(Guid enrollmentId);

    /// <summary>
    /// Returns all Approved commissions for a given period month WITH TRACKING.
    /// Used by the salary lockout job to flip them to Paid.
    /// </summary>
    Task<List<Commission>> GetApprovedByPeriodAsync(DateOnly periodMonth);

    /// <summary>
    /// Returns the OPC commission linked to a specific enrollment WITH TRACKING.
    /// Used to block it when the enrollment is dropped.
    /// </summary>
    Task<Commission?> GetOpcCommissionByEnrollmentAsync(Guid enrollmentId);
}
