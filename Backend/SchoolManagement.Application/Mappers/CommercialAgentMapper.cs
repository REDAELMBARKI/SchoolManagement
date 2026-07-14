using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class CommercialAgentMapper
{
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
}
