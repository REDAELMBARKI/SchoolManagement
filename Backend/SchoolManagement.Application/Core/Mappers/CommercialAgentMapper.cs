using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Core.Mappers;

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
