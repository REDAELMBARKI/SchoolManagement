using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Queries;

public class AbsenceQueryService : IAbsenceQueryService
{
    private readonly AppDbContext _context;

    public AbsenceQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Absence>> GetAllAsync()
    {
        return await _context.Absences
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .Include(a => a.Branch)
            .AsNoTracking()
            .Where(a => a.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<Absence?> GetByIdAsync(Guid id)
    {
        return await _context.Absences
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .Include(a => a.Branch)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Absences
            .AsNoTracking()
            .AnyAsync(a => a.Id == id && a.DeletedAt == null);
    }

    public async Task<List<AbsenceResponseDto>> GetAllResponsesAsync()
    {
        var absences = await GetAllAsync();
        return absences.Select(AbsenceMapper.ToResponse).ToList();
    }

    public async Task<AbsenceResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var absence = await GetByIdAsync(id);
        return absence == null ? null : AbsenceMapper.ToResponse(absence);
    }

    public async Task<List<Absence>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Absences
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .Include(a => a.Branch)
            .AsNoTracking()
            .Where(a => a.StudentId == studentId && a.DeletedAt == null)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public async Task<List<Absence>> GetByScheduleIdAsync(Guid scheduleId)
    {
        return await _context.Absences
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .Include(a => a.Branch)
            .AsNoTracking()
            .Where(a => a.ScheduleId == scheduleId && a.DeletedAt == null)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public async Task<List<Absence>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Absences
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .AsNoTracking()
            .Where(a => a.Date >= startDate && a.Date <= endDate && a.DeletedAt == null)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }
}
