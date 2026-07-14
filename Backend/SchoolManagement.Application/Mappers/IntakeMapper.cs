using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class IntakeMapper
{
    public static Intake ToEntity(IntakeRequestDto dto) => new()
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        Phone = dto.Phone,
        DateOfBirth = dto.DateOfBirth,
        IntakeDate = dto.IntakeDate,
        Status = dto.Status,
        FollowUpDate = dto.FollowUpDate,
        Notes = dto.Notes,
        GenderId = dto.GenderId,
        LeadSourceId = dto.LeadSource.LeadSourceId,
        SubjectId = dto.SubjectId,
        BranchId = dto.BranchId,
        CommercialAgentId = dto.CommercialAgentId,
        IsIndependent = dto.IsIndependent,
        TotalFees = dto.TotalFees,
        AmountPaid = dto.AmountPaid
    };
}
