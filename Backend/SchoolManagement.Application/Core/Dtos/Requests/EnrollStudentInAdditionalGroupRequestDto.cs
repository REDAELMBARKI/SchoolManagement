using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

/// <summary>
/// Request DTO for enrolling an existing student in an additional group/subject.
/// Either PaymentData OR UseCreditBalance must be provided, but not both.
/// </summary>
public class EnrollStudentInAdditionalGroupRequestDto
{
    /// <summary>
    /// The subject to enroll in (required)
    /// </summary>
    [Required(ErrorMessage = "SubjectId is required.")]
    public Guid SubjectId { get; set; }

    /// <summary>
    /// The level for this subject (required)
    /// </summary>
    [Required(ErrorMessage = "LevelId is required.")]
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
    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }

    /// <summary>
    /// Payment option 1: Provide payment details for new payment.
    /// Cannot be used together with UseCreditBalance=true.
    /// </summary>
    public RegistrationPaymentRequestDto? PaymentData { get; set; }

    /// <summary>
    /// Payment option 2: Use student's credit balance.
    /// Cannot be used together with PaymentData.
    /// </summary>
    public bool UseCreditBalance { get; set; }

    /// <summary>
    /// The amount to pay (required).
    /// If UseCreditBalance=true, student must have sufficient credit.
    /// </summary>
    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}
