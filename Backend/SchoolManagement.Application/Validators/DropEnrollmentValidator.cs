using FluentValidation;
using SchoolManagement.Application.Dtos.Requests;

namespace SchoolManagement.Application.Validators;

public class DropEnrollmentValidator : AbstractValidator<DropEnrollmentRequestDto>
{
    public DropEnrollmentValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason for dropping the enrollment is required.")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters.");
    }
}
