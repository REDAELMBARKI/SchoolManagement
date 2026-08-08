 using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Repositories;

public class BranchRepository : Repository<Branch>, IBranchRepository
{
    public BranchRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Set<Branch>()
            .AnyAsync(b => b.Slug == slug && b.DeletedAt == null);
    }
}
