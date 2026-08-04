using FluentValidation;
using SchoolManagement.Application.Core.Dtos.Requests;

namespace SchoolManagement.Application.Core.Validators;

public class StudentResponsableValidator : AbstractValidator<StudentResponsableRequestDto>
{
    public StudentResponsableValidator()
    {
        RuleFor(r => r.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required.")
            .MaximumLength(50)
            .WithMessage("FirstName cannot exceed 50 characters.");

        RuleFor(r => r.LastName)
            .NotEmpty()
            .WithMessage("LastName is required.")
            .MaximumLength(50)
            .WithMessage("LastName cannot exceed 50 characters.");

        RuleFor(r => r.Phone)
            .NotEmpty()
            .WithMessage("Phone is required.")
            .MaximumLength(20)
            .WithMessage("Phone cannot exceed 20 characters.");

        RuleFor(r => r.Email)
            .EmailAddress()
            .When(r => !string.IsNullOrWhiteSpace(r.Email))
            .WithMessage("Invalid email format.");

        RuleFor(r => r.Relationship)
            .IsInEnum()
            .WithMessage("Invalid relationship type.");
    }
}
