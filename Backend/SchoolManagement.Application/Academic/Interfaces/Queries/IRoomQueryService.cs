using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Interfaces.Queries;

public interface IRoomQueryService : IEntityQuery<Room>
{
    Task<List<RoomResponseDto>> GetAllResponsesAsync();
    Task<RoomResponseDto?> GetResponseByIdAsync(Guid id);
    Task<Room?> GetByNameAsync(string name);
    Task<List<Room>> GetAvailableRoomsAsync(int minCapacity);
}
