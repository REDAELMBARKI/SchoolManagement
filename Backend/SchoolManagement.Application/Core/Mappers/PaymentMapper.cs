using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class PaymentMapper
{
    public static Payment ToDomain(RegistrationPaymentCommand command)
    {
        return Payment.Create(
            enrollmentId: command.EnrollmentId,
            amount: command.Amount,
            status: command.Status,
            paidAt: command.PaidAt,
            branchId: command.BranchId,
            receivedByStaffId: command.ReceivedByStaffId,
            invoiceId: command.InvoiceId,
            transferFees: command.TransferFees,
            method: command.Method,
            externalReferenceCode: command.ExternalReferenceCode,
            methodDetailsJson: command.MethodDetailsJson ?? "{}"
        );
    }

    public static Payment ToDomain(ChargeSettlementPaymentCommand command)
    {
        return Payment.Create(
            enrollmentId: command.EnrollmentId,
            amount: command.Amount,
            status: command.Status,
            paidAt: command.PaidAt,
            branchId: command.BranchId,
            receivedByStaffId: command.ReceivedByStaffId,
            invoiceId: command.InvoiceId,
            transferFees: command.TransferFees,
            method: command.Method,
            externalReferenceCode: command.ExternalReferenceCode,
            methodDetailsJson: command.MethodDetailsJson ?? "{}"
        );
    }

    public static PaymentResponseDto ToResponse(Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            EnrollmentId = payment.EnrollmentId,
            InvoiceId = payment.InvoiceId,
            Amount = payment.Amount,
            TransferFees = payment.TransferFees,
            Method = payment.Method,
            Status = payment.Status,
            PaidAt = payment.PaidAt,
            BranchId = payment.BranchId,
            ReceivedByStaffId = payment.ReceivedByStaffId,
            ExternalReferenceCode = payment.ExternalReferenceCode,
            MethodDetailsJson = payment.MethodDetailsJson,
            CurrencyCode = payment.CurrencyCode
        };
    }
}
