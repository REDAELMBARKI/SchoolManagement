using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class IntakeQueryService : QueryServiceBase<Intake>, IIntakeQueryService
{
    public IntakeQueryService(AppDbContext context) : base(context)
    {
    }

    public override async Task<List<Intake>> GetAllAsync()
    {
        return await Query()
            .Include(i => i.Gender)
            .Include(i => i.LeadSource)
            .Include(i => i.Subject)
            .Include(i => i.CommercialAgent)
            .Include(i => i.Branch)
            .ToListAsync();
    }

    public override async Task<Intake?> GetByIdAsync(Guid id)
    {
        return await Query()
            .Include(i => i.Gender)
            .Include(i => i.LeadSource)
            .Include(i => i.Subject)
            .Include(i => i.CommercialAgent)
            .Include(i => i.Branch)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await Query().AnyAsync(i => i.Slug == slug);
    }   


}
