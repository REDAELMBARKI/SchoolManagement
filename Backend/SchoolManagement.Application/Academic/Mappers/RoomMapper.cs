using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Mappers;

public static class RoomMapper
{
    public static Room ToDomain(RoomCommand command, Guid branchId)
    {
        return Room.Create(
            name: command.Name,
            capacity: command.Capacity,
            floor: command.Floor,
            description: command.Description,
            branchId: branchId
        );
    }

    public static RoomResponseDto ToResponse(Room room)
    {
        return new RoomResponseDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Floor = room.Floor,
            Description = room.Description,
            BranchId = room.BranchId,
            CreatedAt = room.CreatedAt
        };
    }
}
