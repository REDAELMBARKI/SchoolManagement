using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

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
            DueDate = charge.DueDate
        };
    }
}