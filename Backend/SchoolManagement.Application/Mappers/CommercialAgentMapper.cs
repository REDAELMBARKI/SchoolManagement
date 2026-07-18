using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class CommercialAgentMapper
{
    public static CommercialAgentResponseDto ToResponse(CommercialAgent commAgent)
    {
        return new CommercialAgentResponseDto
        {
            Id = commAgent.Id,
            Slug = commAgent.Slug,
            FirstName = commAgent.FirstName,
            LastName = commAgent.LastName,
            Email = null, // Email value object not fully implemented
            Phone = commAgent.Phone
        };
    }
}
