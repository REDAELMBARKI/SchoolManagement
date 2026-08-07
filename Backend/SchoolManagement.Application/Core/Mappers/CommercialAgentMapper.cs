using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class CommercialAgentMapper
{
    public static CommercialAgent ToDomain(CommercialAgentCommand command)
    {
        return CommercialAgent.Register(
            firstName: command.FirstName,
            lastName: command.LastName,
            slug: command.Slug,
            genderId: command.GenderId,
            email: command.Email,
            phone: command.Phone,
            dateOfBirth: command.DateOfBirth,
            hireDate: command.HireDate,
            salary: command.Salary,
            branchId: command.BranchId);
    }

    public static CommercialAgentResponseDto ToResponse(CommercialAgent agent)
    {
        return new CommercialAgentResponseDto
        {
            Id = agent.Id,
            FirstName = agent.FirstName,
            LastName = agent.LastName,
            Slug = agent.Slug,
            GenderId = agent.GenderId,
            Email = agent.Email?.Value,
            Phone = agent.Phone,
            DateOfBirth = agent.DateOfBirth,
            HireDate = agent.HireDate,
            Salary = agent.Salary,
            BranchId = agent.BranchId,
            CreatedAt = agent.CreatedAt
        };
    }
}
