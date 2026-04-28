using System;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Mappers;

public class IntakeMapper
{
        public static LeadSourceResponseDto? MapLeadSource(LeadSource? leadSource)
    {
        if(leadSource is null ) return null ;

        if( leadSource != null &&  
            leadSource.AdId != null &&
            leadSource.Ad != null) 
        {
        return  new  AdResponseDto { 
                Id = leadSource.AdId.Value , 
                PlatFormName = leadSource.Ad!.Platform.Name ,
                Type = nameof(Ad) 
                } ;
        }

        if(leadSource != null && 
           leadSource.OpcId != null  &&
           leadSource.Opc != null){
                 
            return  new OpcResponseDto
                {
                    Id = leadSource.OpcId.Value ,
                    FullName = leadSource.Opc!.FirstName + " " + leadSource.Opc.LastName ,
                    Type = nameof(Opc)
                };
           }

        return null ;
    }
   
        public static BranchResponseDto MapBranch(Branch branch)
        {
            return new BranchResponseDto
            {
                Id = branch.Id,
                Slug = branch.Slug,
                Name = branch.Name,
                City = branch.City,
                Address = branch.Address,
                Phone = branch.Phone   
            };
        }

        public static CommercialAgentResponseDto? MapCommercialAgent(CommercialAgent? commAgent)
        {
            if (commAgent is null) return null;
            return new CommercialAgentResponseDto
            {
                Id = commAgent.Id,
                Slug = commAgent.Slug,
                FirstName = commAgent.FirstName,
                LastName = commAgent.LastName,
                Email = commAgent.Email,
                Phone = commAgent.Phone
            };
        }

        public static SchoolProgramResponseDto MapSchoolProgram(SchoolProgram schoolProgram)
        {
            return new SchoolProgramResponseDto
            {
                Id = schoolProgram.Id,
                Slug = schoolProgram.Slug,
                Name = schoolProgram.Name,
                Description = schoolProgram.Description
            };
        }
     public static GenderResponseDto MapGender(Gender gender)
    {
        return new GenderResponseDto
        {  
            Id = gender.Id ,
            Slug = gender.Slug ,
            Name = gender.Name 
        }   ;
    }

    public static Intake ToEntity(IntakeDto dto) => new()
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
        LeadSourceId = dto.LeadSourceId,
        SchoolProgramId = dto.SchoolProgramId,
        BranchId = dto.BranchId,
        CommercialAgentId = dto.CommercialAgentId
    };
}
