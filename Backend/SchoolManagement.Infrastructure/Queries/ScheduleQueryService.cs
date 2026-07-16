using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class ScheduleQueryService : QueryServiceBase<Schedule>, IScheduleQueryService
{
    public ScheduleQueryService(AppDbContext context) : base(context)
    {
    }

    public Task<List<Schedule>> GetAllAsync()
    {
        return Query().ToListAsync();
    }

    public Task<Schedule?> GetByIdAsync(int id)
    {
        return Query().FirstOrDefaultAsync(s => s.Id == id);
    }
}
