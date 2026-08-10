using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Mappers;

public static class UserMapper
{
    public static DomainUser ToDomain(DomainUserCommand command)
    {
        return DomainUser.Register(
            firstName: command.FirstName,
            lastName: command.LastName,
            email: command.Email,
            slug: command.Slug,
            genderId: command.GenderId,
            phone: command.Phone,
            dateOfBirth: command.DateOfBirth,
            role: command.Role,
            branchId: command.BranchId,
            applicationUserId: command.ApplicationUserId
        );
    }

    public static UserResponseDto ToResponse(DomainUser user, string? branchName = null, string? genderName = null)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Slug = user.Slug,
            Email = user.Email?.Value ?? string.Empty,
            Phone = user.Phone,
            DateOfBirth = user.DateOfBirth,
            GenderId = user.GenderId,
            GenderName = genderName,
            Role = user.Role,
            BranchId = user.BranchId,
            BranchName = branchName,
            IsActive = user.IsActive,
            LastActiveAt = user.LastActiveAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
