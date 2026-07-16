using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class EnrollmentQueryService : QueryServiceBase<Enrollment>, IEnrollmentQueryService
{
    public EnrollmentQueryService(AppDbContext context) : base(context)
    {
    }

    public override async Task<List<Enrollment>> GetAllAsync()
    {
        return await Query()
            .Include(e => e.Student)
            .Include(e => e.Subject)
            .Include(e => e.Group)
            .Include(e => e.Branch)
            .Include(e => e.Plan)
            .Include(e => e.Payments)
            .ToListAsync();
    }

    public override async Task<Enrollment?> GetByIdAsync(int id)
    {
        return await Query()
            .Include(e => e.Student)
            .Include(e => e.Subject)
            .Include(e => e.Group)
            .Include(e => e.Branch)
            .Include(e => e.Plan)
            .Include(e => e.Payments)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
