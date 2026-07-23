using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class ChargeMapper
{
    public static Charge ToDomain(ChargeRequestDto dto)
    {
        return Charge.Create(
            studentId: dto.StudentId,
            type: dto.ChargeType,
            amount: dto.Amount,
            issuedDate: dto.IssuedDate,
            dueDate: dto.DueDate,
            description: dto.Description,
            amountPaid: dto.AmountPaid,
            sourceId: dto.SourceId,
            branchId: dto.BranchId
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