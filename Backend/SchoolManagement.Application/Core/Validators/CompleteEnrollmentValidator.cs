using FluentValidation;
using SchoolManagement.Application.Core.Dtos.Requests;

namespace SchoolManagement.Application.Core.Validators;

public class CompleteEnrollmentValidator : AbstractValidator<CompleteEnrollmentRequestDto>
{
    public CompleteEnrollmentValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}
