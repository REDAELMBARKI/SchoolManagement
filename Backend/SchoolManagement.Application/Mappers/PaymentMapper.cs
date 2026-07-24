using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class PaymentMapper
{
    public static Payment ToDomain(PaymentCommand command)
    {
        return Payment.Create(
            enrollmentId: command.EnrollmentId,
            amount: command.Amount,
            status: command.Status,
            paidAt: command.PaidAt,
            branchId: command.BranchId,
            receivedByStaffId: command.ReceivedByStaffId,
            transferFees: command.TransferFees,
            method: command.Method,
            externalReferenceCode: command.ExternalReferenceCode,
            methodDetailsJson: command.MethodDetailsJson
        );
    }

    public static PaymentResponseDto ToResponse(Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            EnrollmentId = payment.EnrollmentId,
            Amount = payment.Amount,
            TransferFees = payment.TransferFees,
            Method = payment.Method,
            Status = payment.Status,
            PaidAt = payment.PaidAt,
            BranchId = payment.BranchId,
            ReceivedByStaffId = payment.ReceivedByStaffId,
            ExternalReferenceCode = payment.ExternalReferenceCode,
            MethodDetailsJson = payment.MethodDetailsJson
        };
    }
}