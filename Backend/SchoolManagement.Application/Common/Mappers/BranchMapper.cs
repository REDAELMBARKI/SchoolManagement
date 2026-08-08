using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Mappers;

public static class BranchMapper
{
    public static Branch ToDomain(BranchCommand command)
    {
        return Branch.Create(
            name: command.Name,
            slug: command.Slug,
            city: command.City,
            address: command.Address,
            phone: command.Phone
        );
    }

    public static BranchResponseDto ToResponse(Branch branch)
    {
        return new BranchResponseDto
        {
            Id = branch.Id,
            Slug = branch.Slug,
            Name = branch.Name,
            City = branch.City,
            Address = branch.Address,
            Phone = branch.Phone
        };
    }
}
