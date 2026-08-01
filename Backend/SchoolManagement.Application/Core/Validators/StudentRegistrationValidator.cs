using FluentValidation;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Core.Validators;

public class StudentRegistrationValidator : AbstractValidator<StudentRegistrationRequestDto>
{
    public StudentRegistrationValidator(
        IValidator<StudentRequestDto> studentValidator,
        IValidator<EnrollmentRequestDto> enrollmentValidator)
    {
        RuleFor(x => x.StudentRegReq)
            .NotNull().WithMessage("Student information is required.")
            .SetValidator(studentValidator);

        RuleFor(x => x.EnrollmentRegReq)
            .NotNull().WithMessage("Enrollment information is required.")
            .SetValidator(enrollmentValidator);

        RuleFor(x => x.PaymentRegReq)
            .NotNull().WithMessage("Payment information is required.");

        When(x => x.PaymentRegReq != null, () =>
        {
            RuleFor(x => x.PaymentRegReq!.AmountPaid)
                .GreaterThan(0).WithMessage("Paid amount must be greater than zero.");

            RuleFor(x => x.PaymentRegReq!.TransferFees)
                .GreaterThanOrEqualTo(0).When(x => x.PaymentRegReq!.TransferFees.HasValue)
                .WithMessage("Transfer fees cannot be negative.");

            RuleFor(x => x.PaymentRegReq!.Method)
                .IsInEnum().WithMessage("Invalid payment method.");

            RuleFor(x => x.PaymentRegReq!.ExternalReferenceCode)
                .NotEmpty()
                .When(x => x.PaymentRegReq!.Method == PaymentMethod.BankTransfer
                        || x.PaymentRegReq!.Method == PaymentMethod.CreditCard
                        || x.PaymentRegReq!.Method == PaymentMethod.Check)
                .WithMessage("External reference code is required for bank/credit/check payments.");
        });
    }
}
