using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class StudentResponsableRepository : Repository<StudentResponsable>, IStudentResponsableRepository
{
    public StudentResponsableRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> IsExistsBySlugAsync(string slug)
    {
        return await _context.Set<StudentResponsable>()
            .Where(sr => EF.Property<DateTime?>(sr, "DeletedAt") == null)
            .AnyAsync(sr => sr.Slug == slug);
    }
}
