using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Mappers;

public static class IntakeMapper
{
    public static Intake ToDomain(IntakeCommand command)
    {  
        return Intake.Register(
            firstName: command.FirstName,
            lastName: command.LastName,
            slug: command.Slug,
            genderId: command.GenderId,
            email: command.Email,
            phone: command.Phone,
            dateOfBirth: command.DateOfBirth,
            intakeDate: command.IntakeDate,
            status: command.Status,
            followUpDate: command.FollowUpDate,
            notes: command.Notes,
            commercialAgentId: command.CommercialAgentId,
            leadSourceId: command.LeadSourceId,
            subjectId: command.SubjectId,
            branchId: command.BranchId,
            isIndependent: command.IsIndependent,
            totalFees: command.TotalFees,
            amountPaid: command.AmountPaid
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
            Email = intake.Email?.Value ?? null, 
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

        foreach (var student in intake.Students)
        {
            response.Students.Add(new StudentResponseDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Slug = student.Slug,
                Email = student.Email?.Value ?? null,
                Phone = student.Phone,
                DateOfBirth = student.DateOfBirth,
                IntakeId = student.IntakeId,
                IsDirectRegistration = student.IsDirectRegistration
            });
        }

        return response;
    }
}
