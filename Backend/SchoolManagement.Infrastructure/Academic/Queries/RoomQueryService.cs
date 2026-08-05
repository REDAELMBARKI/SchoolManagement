using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Academic.Interfaces.Queries;
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

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _context.Rooms
            .Include(r => r.Branch)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Room>> GetAllAsync()
    {
        var query = _context.Rooms
            .Include(r => r.Branch)
            .AsQueryable();

     

        return await query
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Room?> GetByNameAsync(string name, Guid branchId)
    {
        return await _context.Rooms
            .Include(r => r.Branch)
            .FirstOrDefaultAsync(r => r.Name == name && r.BranchId == branchId);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
       return await _context.Rooms.AnyAsync(r => r.Id == id);
    }
}
