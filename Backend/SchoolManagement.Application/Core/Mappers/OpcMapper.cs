using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class OpcMapper
{
    public static Opc ToDomain(OpcCommand command)
    {
        return Opc.Register(
            firstName: command.FirstName,
            lastName: command.LastName,
            slug: command.Slug,
            genderId: command.GenderId,
            email: command.Email,
            phone: command.Phone,
            dateOfBirth: command.DateOfBirth,
            hireDate: command.HireDate,
            salary: command.Salary,
            branchId: command.BranchId
        );
    }

    public static OpcResponseDto ToResponse(Opc opc)
    {
        return new OpcResponseDto
        {
            Id = opc.Id,
            FirstName = opc.FirstName,
            LastName = opc.LastName,
            Slug = opc.Slug,
            GenderId = opc.GenderId,
            Email = opc.Email?.Value,
            Phone = opc.Phone,
            DateOfBirth = opc.DateOfBirth,
            HireDate = opc.HireDate,
            Salary = opc.Salary,
            BranchId = opc.BranchId,
            CreatedAt = opc.CreatedAt
        };
    }
}
