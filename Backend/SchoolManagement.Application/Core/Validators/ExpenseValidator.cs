using FluentValidation;
using SchoolManagement.Application.Core.Dtos.Requests;

namespace SchoolManagement.Application.Core.Validators;

public class ExpenseValidator : AbstractValidator<ExpenseRequestDto>
{
    public ExpenseValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("A valid expense category is required.");

        RuleFor(x => x.PayeeName)
            .NotEmpty()
            .WithMessage("Payee name is required.")
            .MinimumLength(2)
            .WithMessage("Payee name must be at least 2 characters.")
            .MaximumLength(200)
            .WithMessage("Payee name cannot exceed 200 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.ExpenseDate)
            .NotEmpty()
            .WithMessage("Expense date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Expense date cannot be too far in the future.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .WithMessage("A valid payment method is required.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Reference)
            .MaximumLength(100)
            .WithMessage("Reference cannot exceed 100 characters.");
    }
}
