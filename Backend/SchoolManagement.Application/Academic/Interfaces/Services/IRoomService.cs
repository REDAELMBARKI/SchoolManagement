using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;

namespace SchoolManagement.Application.Academic.Interfaces.Services;

public interface IRoomService
{
    Task<List<RoomResponseDto>> GetAllAsync();
    Task<RoomResponseDto> GetByIdAsync(Guid id);
    Task<RoomResponseDto> CreateAsync(RoomCommand command);
    Task<RoomResponseDto> UpdateAsync(Guid id, UpdateRoomCommand command);
    Task DeleteAsync(Guid id);
}
