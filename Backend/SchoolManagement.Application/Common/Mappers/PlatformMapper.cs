using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Mappers;

public static class PlatformMapper
{
    public static Platform ToDomain(PlatformCommand command)
    {
        return Platform.Create(
            name: command.Name,
            slug: command.Slug,
            branchId: command.BranchId);
    }

    public static PlatformResponseDto ToResponse(Platform platform)
    {
        return new PlatformResponseDto
        {
            Id = platform.Id,
            Name = platform.Name,
            Slug = platform.Slug,
            BranchId = platform.BranchId,
            CreatedAt = platform.CreatedAt
        };
    }
}
