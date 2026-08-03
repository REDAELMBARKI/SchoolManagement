using SchoolManagement.Application.Core.Dtos.Requests;

namespace SchoolManagement.Application.Core.Dtos.Commands;

/// <summary>
/// Command for enrolling an existing student in an additional group/subject.
/// Used when a student already exists and has at least one enrollment,
/// and wants to enroll in another subject (e.g., adding Math when already taking English).
/// </summary>
public class EnrollStudentInAdditionalGroupCommand
{
    /// <summary>
    /// The existing student's ID
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// The subject to enroll in
    /// </summary>
    public Guid SubjectId { get; set; }

    /// <summary>
    /// The level for this subject
    /// </summary>
    public Guid LevelId { get; set; }

    /// <summary>
    /// Optional: Specific group to enroll in. If null, system will auto-select best group.
    /// </summary>
    public Guid? GroupId { get; set; }

    /// <summary>
    /// Optional: Preferred schedule for group selection
    /// </summary>
    public Guid? PreferredScheduleId { get; set; }

    /// <summary>
    /// Optional: Payment plan to apply
    /// </summary>
    public Guid? PlanId { get; set; }

    /// <summary>
    /// Optional: Notes about this enrollment
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Payment option 1: Provide payment details for new payment
    /// Cannot be used together with UseCreditBalance
    /// </summary>
    public RegistrationPaymentRequestDto? PaymentData { get; set; }

    /// <summary>
    /// Payment option 2: Use student's credit balance
    /// Cannot be used together with PaymentData
    /// </summary>
    public bool UseCreditBalance { get; set; }

    /// <summary>
    /// The amount to pay (either via PaymentData or CreditBalance)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Branch context (set by service from current user context)
    /// </summary>
    public Guid BranchId { get; set; }
}
