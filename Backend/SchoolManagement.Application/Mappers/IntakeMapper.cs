using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class IntakeMapper
{
    public static Intake ToDomain(IntakeRequestDto dto)
    {
        return Intake.Register(
            firstName: dto.FirstName,
            lastName: dto.LastName,
            genderId: dto.GenderId,
            email: dto.Email,
            phone: dto.Phone,
            dateOfBirth: dto.DateOfBirth,
            intakeDate: dto.IntakeDate,
            status: dto.Status,
            followUpDate: dto.FollowUpDate,
            notes: dto.Notes,
            commercialAgentId: dto.CommercialAgentId,
            leadSourceId: dto.LeadSource.LeadSourceId,
            subjectId: dto.SubjectId,
            branchId: dto.BranchId,
            isIndependent: dto.IsIndependent,
            totalFees: dto.TotalFees,
            amountPaid: dto.AmountPaid
        );
    }

    public static IntakeResponseDto ToResponse(Intake intake)
    {
        var response = new IntakeResponseDto
        {
            Id = intake.Id,
            FirstName = intake.FirstName,
            LastName = intake.LastName,
            Slug = intake.Slug,
            Email = null, 
            Phone = intake.Phone,
            IntakeDate = intake.IntakeDate,
            DateOfBirth = intake.DateOfBirth,
            FollowUpDate = intake.FollowUpDate,
            CreatedAt = intake.CreatedAt,
            Notes = intake.Notes,
            Status = intake.Status,
            IsIndependent = intake.IsIndependent,
            TotalFees = intake.TotalFees,
            AmountPaid = intake.AmountPaid,
            Subject = intake.Subject != null ? new SubjectResponseDto
            {
                Id = intake.Subject.Id,
                Name = intake.Subject.Name,
                Slug = intake.Subject.Slug,
                Description = intake.Subject.Description,
                BranchId = intake.Subject.BranchId
            } : null!,
            Branch = intake.Branch != null ? new BranchResponseDto
            {
                Id = intake.Branch.Id,
                Name = intake.Branch.Name,
                Slug = intake.Branch.Slug,
                City = intake.Branch.City,
                Address = intake.Branch.Address,
                Phone = intake.Branch.Phone
            } : null!,
            Gender = intake.Gender != null ? new GenderResponseDto
            {
                Id = intake.Gender.Id,
                Slug = intake.Gender.Slug,
                Name = intake.Gender.Name
            } : null!,
            CommercialAgent = intake.CommercialAgent != null ? new CommercialAgentResponseDto
            {
                Id = intake.CommercialAgent.Id,
                FirstName = intake.CommercialAgent.FirstName,
                LastName = intake.CommercialAgent.LastName,
                Slug = intake.CommercialAgent.Slug,
                Email = null,
                Phone = intake.CommercialAgent.Phone
            } : null
        };

        // Handle LeadSource mapping
        if (intake.LeadSource is OpcLeadSource opcLeadSource)
        {
            response.LeadSource = new OpcResponseDto
            {
                Id = opcLeadSource.Id,
                Type = "Opc",
                FullName = opcLeadSource.Opc != null ? $"{opcLeadSource.Opc.FirstName} {opcLeadSource.Opc.LastName}" : string.Empty
            };
        }
        else if (intake.LeadSource is AdLeadSource adLeadSource)
        {
            response.LeadSource = new AdResponseDto
            {
                Id = adLeadSource.Id,
                Type = "Ad",
                PlatFormName = adLeadSource.Ad?.Platform?.Name ?? string.Empty
            };
        }

        // Handle ConvertedToStudent
        if (intake.ConvertedToStudent != null)
        {
            response.ConvertedToStudent = new StudentResponseDto
            {
                Id = intake.ConvertedToStudent.Id,
                FirstName = intake.ConvertedToStudent.FirstName,
                LastName = intake.ConvertedToStudent.LastName,
                Slug = intake.ConvertedToStudent.Slug,
                Email = intake.ConvertedToStudent.Email,
                Phone = intake.ConvertedToStudent.Phone,
                DateOfBirth = intake.ConvertedToStudent.DateOfBirth,
                IntakeId = intake.ConvertedToStudent.IntakeId
            };
        }

        return response;
    }
}
