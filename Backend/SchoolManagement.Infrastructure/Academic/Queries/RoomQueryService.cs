using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Queries;

public class RoomQueryService : IRoomQueryService
{
    private readonly AppDbContext _context;

    public RoomQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.Branch)
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _context.Rooms
            .Include(r => r.Branch)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Rooms
            .AsNoTracking()
            .AnyAsync(r => r.Id == id);
    }

    public async Task<List<RoomResponseDto>> GetAllResponsesAsync()
    {
        var rooms = await GetAllAsync();
        return rooms.Select(RoomMapper.ToResponse).ToList();
    }

    public async Task<RoomResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var room = await GetByIdAsync(id);
        return room == null ? null : RoomMapper.ToResponse(room);
    }

    public async Task<Room?> GetByNameAsync(string name)
    {
        return await _context.Rooms
            .Include(r => r.Branch)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<List<Room>> GetAvailableRoomsAsync(int minCapacity)
    {
        return await _context.Rooms
            .Include(r => r.Branch)
            .AsNoTracking()
            .Where(r => r.Capacity >= minCapacity)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }
}
