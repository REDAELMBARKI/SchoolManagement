using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Repositories;

public class SubjectRepository : Repository<Subject>, ISubjectRepository
{
    public SubjectRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Set<Subject>()
            .AnyAsync(s => s.Slug == slug && s.DeletedAt == null);
    }
}
