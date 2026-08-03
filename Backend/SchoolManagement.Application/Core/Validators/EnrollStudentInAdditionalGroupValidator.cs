using FluentValidation;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Validators;

/// <summary>
/// Validator for EnrollStudentInAdditionalGroupCommand.
/// Ensures payment method is properly specified and student/subject/level are provided.
/// </summary>
public class EnrollStudentInAdditionalGroupValidator : AbstractValidator<EnrollStudentInAdditionalGroupCommand>
{
    private readonly IStudentQueryService _studentQueryService;

    public EnrollStudentInAdditionalGroupValidator(IStudentQueryService studentQueryService)
    {
        _studentQueryService = studentQueryService;

        // Required fields
        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("StudentId is required.")
            .MustAsync(async (studentId, cancellation) => 
            {
                var student = await _studentQueryService.FindByIdAsync(studentId);
                return student != null;
            })
            .WithMessage("Student with the specified ID does not exist.");

        RuleFor(x => x.SubjectId)
            .NotEmpty()
            .WithMessage("SubjectId is required.");

        RuleFor(x => x.LevelId)
            .NotEmpty()
            .WithMessage("LevelId is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        // Optional fields with constraints
        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters.");

        // Payment method validation: Must provide ONE payment option
        RuleFor(x => x)
            .Must(HaveValidPaymentOption)
            .WithMessage("Either PaymentData OR UseCreditBalance must be provided, but not both.")
            .WithName("PaymentOption");

        // If using credit balance, validate student has enough (this will be checked in service too)
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .When(x => x.UseCreditBalance)
            .WithMessage("Amount must be specified when using credit balance.");

        // If providing payment data, validate it's complete
        RuleFor(x => x.PaymentData)
            .NotNull()
            .When(x => !x.UseCreditBalance && x.PaymentData != null)
            .WithMessage("PaymentData must be complete when provided.");

        RuleFor(x => x.PaymentData!.AmountPaid)
            .Equal(x => x.Amount)
            .When(x => x.PaymentData != null)
            .WithMessage("PaymentData.AmountPaid must match the specified Amount.");
    }

    /// <summary>
    /// Validates that exactly ONE payment option is provided (not both, not neither)
    /// </summary>
    private bool HaveValidPaymentOption(EnrollStudentInAdditionalGroupCommand command)
    {
        bool hasPaymentData = command.PaymentData != null;
        bool usesCreditBalance = command.UseCreditBalance;

        // XOR: exactly one must be true
        return hasPaymentData ^ usesCreditBalance;
    }
}
