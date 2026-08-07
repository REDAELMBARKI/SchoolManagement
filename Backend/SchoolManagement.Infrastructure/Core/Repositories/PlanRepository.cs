using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class PlanRepository : Repository<Plan>, IPlanRepository
{
    public PlanRepository(AppDbContext context) : base(context) { }

    public async Task<List<Plan>> GetActiveAsync()
    {
        return await Query()
            .Where(p => p.IsActive)
            .OrderBy(p => p.DurationMonths)
            .ToListAsync();
    }
}
