using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class GroupQueryService : QueryServiceBase<Group>, IGroupQueryService
{
    public GroupQueryService(AppDbContext context) : base(context)
    {
    }

    public override async Task<List<Group>> GetAllAsync()
    {
        return await Query()
            .Include(g => g.Level)
            .Include(g => g.Subject)
            .Include(g => g.Teachers)
            .ToListAsync();
    }

    public override async Task<Group?> GetByIdAsync(int id)
    {
        return await Query()
            .Include(g => g.Level)
            .Include(g => g.Subject)
            .Include(g => g.Teachers)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}
