using FluentValidation;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Requests;

namespace SchoolManagement.Application.Core.Validators;

public class WaiveInvoiceValidator : AbstractValidator<WaiveInvoiceRequestDto>
{
    public WaiveInvoiceValidator()
    {
        RuleFor(x => x.WaivedAmount)
            .GreaterThan(0)
            .WithMessage("Waived amount must be greater than zero.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason for waiver is required.")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters.");
    }
}
