using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class AdMapper
{
    public static Ad ToDomain(AdCommand command)
    {
        return Ad.Create(
            name: command.Name,
            slug: command.Slug,
            platformId: command.PlatformId,
            branchId: command.BranchId
        );
    }

    public static AdResponseDto ToResponse(Ad ad)
    {
        return new AdResponseDto
        {
            Id = ad.Id,
            Name = ad.Name,
            Slug = ad.Slug,
            PlatformId = ad.PlatformId,
            BranchId = ad.BranchId,
            CreatedAt = ad.CreatedAt
        };
    }
}
