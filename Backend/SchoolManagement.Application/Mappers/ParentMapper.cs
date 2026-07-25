using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class ParentMapper
{
    public static Parent ToDomain(ParentCommand command)
    {
        return Parent.Register(
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

    public static ParentResponseDto ToResponse(Parent parent)
    {
        return new ParentResponseDto
        {
            Id = parent.Id,
            FirstName = parent.FirstName,
            LastName = parent.LastName,
            Slug = parent.Slug,
            Email = parent.Email,
            Phone = parent.Phone,
            Relationship = parent.Relationship.ToString(),
            Gender = parent.Gender != null ? new GenderResponseDto
            {
                Id = parent.Gender.Id,
                Name = parent.Gender.Name
            } : null
        };
    }
}
