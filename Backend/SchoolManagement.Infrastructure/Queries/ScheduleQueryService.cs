using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class ScheduleQueryService : IScheduleQueryService
{
    private readonly AppDbContext _context;

    public ScheduleQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Schedule>> GetAllAsync()
    {
        return await _context.Schedules
            .Include(s => s.Teacher)
            .Include(s => s.Room)
            .Include(s => s.Group)
            .Include(s => s.Subject)
            .Include(s => s.Branch)
            .Where(s => EF.Property<DateTime?>(s, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<Schedule?> GetByIdAsync(Guid id)
    {
        return await _context.Schedules
            .Include(s => s.Teacher)
            .Include(s => s.Room)
            .Include(s => s.Group)
            .Include(s => s.Subject)
            .Include(s => s.Branch)
            .Where(s => EF.Property<DateTime?>(s, "DeletedAt") == null)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Schedules
            .Where(s => EF.Property<DateTime?>(s, "DeletedAt") == null)
            .AnyAsync(s => s.Id == id);
    }

    public async Task<List<ScheduleResponseDto>> GetAllResponsesAsync()
    {
        var schedules = await GetAllAsync();
        return schedules.Select(ScheduleMapper.ToResponse).ToList();
    }

    public async Task<ScheduleResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var schedule = await GetByIdAsync(id);
        return schedule == null ? null : ScheduleMapper.ToResponse(schedule);
    }



}
