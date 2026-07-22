using FluentValidation;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Domain.Interfaces.Queries;

namespace SchoolManagement.Application.Validators;

public class StudentValidator : AbstractValidator<StudentRequestDto>
{
    public StudentValidator(IIntakeQueryService intakeQueryService)
    {
        // Basic property validations
        RuleFor(s => s.FirstName).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(s => s.LastName).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(s => s.Email).EmailAddress().MaximumLength(255).When(s => !string.IsNullOrEmpty(s.Email));
        RuleFor(s => s.Phone).NotEmpty().MaximumLength(20);
        RuleFor(s => s.DateOfBirth).NotEmpty();

        // Validate either IntakeId is provided OR IsDirectRegistration is true
        RuleFor(s => s)
            .Must(s => s.IntakeId.HasValue || s.IsDirectRegistration)
            .WithMessage("Either IntakeId must be provided or IsDirectRegistration must be true.");

        // Validate that both IntakeId and IsDirectRegistration are not set
        RuleFor(s => s)
            .Must(s => !s.IntakeId.HasValue || !s.IsDirectRegistration)
            .WithMessage("Cannot set both IntakeId and IsDirectRegistration to true.");

        // Validate that IntakeId exists if provided
        RuleFor(s => s.IntakeId)
            .MustAsync(async (intakeId, ct) => 
            {
                if (!intakeId.HasValue) return true;
                return await intakeQueryService.IsExistsAsync(intakeId.Value);
            })
            .When(s => s.IntakeId.HasValue)
            .WithMessage("Selected intake does not exist.");
    }
}
