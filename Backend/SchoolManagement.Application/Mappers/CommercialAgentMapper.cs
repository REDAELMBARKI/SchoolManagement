using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class CommercialAgentMapper
{
    public static CommercialAgent ToDomain(CommercialAgentResponseDto dto, DateTime hireDate, decimal salary, Guid branchId, DateOnly? dateOfBirth = null, Guid? genderId = null)
    {
        return CommercialAgent.Register(
            firstName: dto.FirstName,
            lastName: dto.LastName,
            slug: dto.Slug,
            genderId: genderId,
            email: dto.Email,
            phone: dto.Phone,
            dateOfBirth: dateOfBirth,
            hireDate: hireDate,
            salary: salary,
            branchId: branchId
        );
    }

    public static CommercialAgentResponseDto ToResponse(CommercialAgent commAgent)
    {
        return new CommercialAgentResponseDto
        {
            Id = commAgent.Id,
            Slug = commAgent.Slug,
            FirstName = commAgent.FirstName,
            LastName = commAgent.LastName,
            Email = commAgent.Email?.Value ?? null,
            Phone = commAgent.Phone
        };
    }
}
