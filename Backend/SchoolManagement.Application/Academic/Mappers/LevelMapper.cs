using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Mappers;

public static class LevelMapper
{
    public static Level ToDomain(LevelCommand command, Guid branchId)
    {
        return Level.Create(
            name: command.Name,
            branchId: branchId,
            order: command.Order
        );
    }

    public static LevelResponseDto ToResponse(Level level)
    {
        return new LevelResponseDto
        {
            Id = level.Id,
            Name = level.Name,
            Order = level.Order,
            BranchId = level.BranchId,
            CreatedAt = level.CreatedAt
        };
    }
}
