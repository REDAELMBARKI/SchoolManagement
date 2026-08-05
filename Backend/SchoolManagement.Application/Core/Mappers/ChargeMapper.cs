using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class ChargeMapper
{
    public static Charge ToDomain(ChargeCommand command)
    {
        return Charge.Create(
            invoiceId: command.InvoiceId,
            amount: command.Amount,
            dueDate: command.DueDate
        );
    }

    public static ChargeResponseDto ToResponse(Charge charge)
    {
        return new ChargeResponseDto
        {
            Id = charge.Id,
            InvoiceId = charge.InvoiceId,
            Amount = charge.Amount,
            PaidAmount = charge.PaidAmount,
            WaivedAmount = charge.WaivedAmount,
            WaivedReason = charge.WaivedReason,
            DueDate = charge.DueDate,
            Status = charge.Status.ToString()
        };
    }
}
