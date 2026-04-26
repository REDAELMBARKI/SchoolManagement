using System;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Mappers;

public class IntakeMapper
{
   // Service — does the mapping using GetLeadSource()
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
   
   public static BranchResponseDto? MapBranch(Branch? branch)
    {
        
    }

     public static CommercialAgentResponseDto? MapCommercialAgent(CommercialAgent? commAgent)
    {
        
    }

     public static SchoolProgramResponseDto? MapSchoolProgram(SchoolProgram? schoolProgram)
    {
        
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

   public static Intake ToEntity(IntakeDto intakeDto)
   {
        return new Intake
        {
            FirstName = intakeDto.FirstName ,
            LastName  = intakeDto.FirstName , 
            IntakeDate = intakeDto.IntakeDate ,
            Phone = intakeDto.Phone ,
            Email = intakeDto.Email ,
            GenderId = intakeDto.GenderId ,
            LeadSourceId = intakeDto.LeadSourceId , 
            
         };
   }
}
