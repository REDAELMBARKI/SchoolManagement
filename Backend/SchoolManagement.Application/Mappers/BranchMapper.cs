using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class BranchMapper : IMapper<Branch>
{

    public static Branch MapToEntity<BranchResponseDto>(BranchResponseDto dto)
    {
        throw new NotImplementedException();
    }

    public static BranchResponseDto MapToResponseDto<BranchResponseDto>(Branch branch)
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
