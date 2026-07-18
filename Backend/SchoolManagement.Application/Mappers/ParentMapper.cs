using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class ParentMapper
{
    public static Parent ToDomain(ParentRequestDto dto)
    {
        var relationship = Enum.TryParse<RelationshipType>(dto.Relationship, true, out var rel)
            ? rel
            : RelationshipType.Other;

        return Parent.Register(
            firstName: dto.FirstName,
            lastName: dto.LastName,
            slug: dto.Slug,
            genderId: dto.GenderId,
            email: dto.Email,
            phone: dto.Phone,
            relationship: relationship
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
