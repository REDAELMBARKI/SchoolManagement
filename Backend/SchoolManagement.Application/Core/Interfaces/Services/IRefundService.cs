using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IRefundService
{
    /// <summary>
    /// Records a cash refund against a payment.
    /// Reduces the linked invoice's PaidAmount and recalculates its status.
    /// If the payment is fully refunded, marks it as Refunded.
    /// </summary>
    Task<RefundResponseDto> RefundPaymentAsync(Guid paymentId, RefundCommand command);

    /// <summary>Returns all refunds recorded against a specific payment.</summary>
    Task<List<RefundResponseDto>> GetByPaymentIdAsync(Guid paymentId);
}
