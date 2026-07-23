using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class PaymentMapper
{
    public static Payment ToDomain(PaymentRequestDto dto)
    {
        return Payment.Create(
            enrollmentId: dto.EnrollmentId,
            feeAmount: dto.FeeAmount,
            periodStart: dto.PeriodStart,
            periodEnd: dto.PeriodEnd,
            amountPaid: dto.AmountPaid,
            paidAt: dto.PaidAt,
            status: dto.Status
        );
    }

    public static PaymentResponseDto ToResponse(Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            EnrollmentId = payment.EnrollmentId,
            FeeAmount = payment.FeeAmount,
            AmountPaid = payment.AmountPaid,
            PeriodStart = payment.PeriodStart,
            PeriodEnd = payment.PeriodEnd,
            Status = payment.Status,
            PaidAt = payment.PaidAt
        };
    }
}