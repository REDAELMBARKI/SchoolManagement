using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Mappers;

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
