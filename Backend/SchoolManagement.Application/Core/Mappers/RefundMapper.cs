using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class RefundMapper
{
    public static Refund ToDomain(RefundCommand command, Guid paymentId)
    {
        return Refund.Create(
            paymentId: paymentId,
            amount: command.Amount,
            reason: command.Reason,
            refundedByStaffId: command.RefundedByStaffId,
            branchId: command.BranchId
        );
    }

    public static RefundResponseDto ToResponse(Refund refund)
    {
        return new RefundResponseDto
        {
            Id = refund.Id,
            PaymentId = refund.PaymentId,
            Amount = refund.Amount,
            Reason = refund.Reason,
            RefundedAt = refund.RefundedAt,
            RefundedByStaffId = refund.RefundedByStaffId,
            BranchId = refund.BranchId,
            CreatedAt = refund.CreatedAt
        };
    }
}
