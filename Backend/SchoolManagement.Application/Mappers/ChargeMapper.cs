using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class ChargeMapper
{
    public static Charge ToDomain(ChargeCommand command)
    {
        return Charge.Create(
            studentId: command.StudentId,
            type: command.ChargeType,
            amount: command.Amount,
            issuedDate: command.IssuedDate,
            dueDate: command.DueDate,
            amountPaid: command.AmountPaid,
            sourceId: command.SourceId,
            branchId: command.BranchId
        );
    }

    public static ChargeResponseDto ToResponse(Charge charge)
    {
        return new ChargeResponseDto
        {
            Id = charge.Id,
            StudentId = charge.StudentId,
            ChargeType = charge.ChargeType,
            Description = charge.Description,
            Amount = charge.Amount,
            AmountPaid = charge.AmountPaid,
            Status = charge.Status,
            IssuedDate = charge.IssuedDate,
            DueDate = charge.DueDate,
            SourceId = charge.SourceId,
            BranchId = charge.BranchId
        };
    }
}