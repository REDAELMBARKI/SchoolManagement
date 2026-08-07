using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface ICommissionService
{
    /// <summary>
    /// Called by the OPC event handler when a new enrollment is created.
    /// Looks up the OPC via Enrollment → Student → Intake → OpcLeadSource and creates a commission.
    /// </summary>
    Task ProcessOpcCommissionAsync(Guid enrollmentId, Guid studentId);

    /// <summary>
    /// Called when an enrollment is dropped — auto-blocks the linked OPC commission
    /// if the salary lockout hasn't passed yet.
    /// </summary>
    Task BlockOpcCommissionByEnrollmentAsync(Guid enrollmentId, string reason);

    /// <summary>
    /// Runs at end of month. Counts each agent's enrollments for the given month,
    /// resolves their tier, and records a commission. Skips agents already calculated.
    /// </summary>
    Task ProcessAgentMonthlyCommissionsAsync(int year, int month);

    /// <summary>
    /// Salary day lockout job — flips all Approved commissions for the current period
    /// to Paid. Blocked ones stay Blocked. After this nothing can change.
    /// </summary>
    Task ProcessSalaryLockoutAsync(int year, int month);

    /// <summary>
    /// Manually block a commission (e.g. manager decision or enrollment dropped).
    /// Only allowed before the salary lockout date.
    /// </summary>
    Task<CommissionResponseDto> BlockCommissionAsync(Guid id, string reason);

    Task<List<CommissionResponseDto>> GetByEarnerAsync(Guid earnerId, EarnerType earnerType);
    Task<List<CommissionResponseDto>> GetByPeriodAsync(int year, int month);
    Task<CommissionResponseDto> GetByIdAsync(Guid id);
    Task<List<CommissionResponseDto>> GetAllAsync();
    Task<CommissionResponseDto> ApproveAsync(Guid id);
    Task<CommissionResponseDto> MarkAsPaidAsync(Guid id);
}
