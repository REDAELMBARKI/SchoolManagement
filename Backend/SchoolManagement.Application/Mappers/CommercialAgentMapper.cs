using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public  class CommercialAgentMapper 
{
    public CommercialAgent MapToEntity<CommercialAgentRequestDto>(CommercialAgentRequestDto requestDto)
    {
        throw new NotImplementedException();
    }

    public  CommercialAgentResponseDto MapToResponseDto(CommercialAgent? commAgent) 
    {
        return new CommercialAgentResponseDto
        {
            Id = commAgent.Id,
            Slug = commAgent.Slug,
            FirstName = commAgent.FirstName,
            LastName = commAgent.LastName,
            Email = commAgent.Email ,
            Phone = commAgent.Phone
        };
    }

  
}
