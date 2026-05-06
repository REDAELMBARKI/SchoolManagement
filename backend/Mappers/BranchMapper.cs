using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Mappers;

public static class BranchMapper
{
    public static BranchResponseDto MapBranch(Branch branch)
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
