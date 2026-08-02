using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class StudentResponsableMapper
{
    public static StudentResponsable ToDomain(StudentResponsableCommand command)
    {
        return StudentResponsable.Register(
            firstName: command.FirstName,
            lastName: command.LastName,
            slug: command.Slug,
            genderId: command.GenderId,
            email: command.Email,
            phone: command.Phone,
            relationship: command.Relationship,
            branchId: command.BranchId
        );
    }

    public static StudentResponsableResponseDto ToResponse(StudentResponsable responsable)
    {
        return new StudentResponsableResponseDto
        {
            Id = responsable.Id,
            FirstName = responsable.FirstName,
            LastName = responsable.LastName,
            Slug = responsable.Slug,
            Email = responsable.Email,
            Phone = responsable.Phone,
            Relationship = responsable.Relationship.ToString(),
            Gender = responsable.Gender != null ? new GenderResponseDto
            {
                Id = responsable.Gender.Id,
                Name = responsable.Gender.Name
            } : null
        };
    }
}
