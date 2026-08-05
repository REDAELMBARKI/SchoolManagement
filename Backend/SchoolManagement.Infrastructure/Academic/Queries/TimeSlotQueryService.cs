using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Queries;

public class TimeSlotQueryService : ITimeSlotQueryService
{
    private readonly AppDbContext _context;

    public TimeSlotQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TimeSlot?> GetByIdAsync(Guid id)
    {
        return await _context.TimeSlots
            .FirstOrDefaultAsync(ts => ts.Id == id);
    }

    public async Task<List<TimeSlot>> GetAllAsync()
    {
        return await _context.TimeSlots
            .OrderBy(ts => ts.StartTime)
            .ToListAsync();
    }

    public async Task<TimeSlot?> GetByTimesAsync(TimeOnly startTime, TimeOnly endTime)
    {
        return await _context.TimeSlots
            .FirstOrDefaultAsync(ts => ts.StartTime == startTime && ts.EndTime == endTime);
    }
}
