using FluentValidation;
using SchoolManagement.Application.Dtos.Commands;

namespace SchoolManagement.Application.Validators;

public class CancelInvoiceValidator : AbstractValidator<CancelInvoiceCommand>
{
    public CancelInvoiceValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason for cancellation is required.")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters.");
    }
}
