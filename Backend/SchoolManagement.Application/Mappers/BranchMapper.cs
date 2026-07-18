using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class BranchMapper
{
    public static Branch ToDomain(BranchRequestDto dto)
    {
        return Branch.Create(
            name: dto.Name,
            slug: dto.Slug,
            city: dto.City,
            address: dto.Address,
            phone: dto.Phone
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
