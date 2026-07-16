using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class IntakeRepository : Repository<Intake>, IIntakeRepository
{
    public IntakeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Intake> AddAsync(Intake intake)
    {
        return await base.AddAsync(intake);
    }

    public async Task UpdateAsync(Intake intake)
    {
        await base.UpdateAsync(intake);
    }

    public async Task DeleteAsync(Guid id) 
    {
        await base.DeleteAsync(id);
    }
    
    public  async Task<Intake?> GetByIdForTrackingAsync(Guid id)
    {
        return await base.GetByIdForTrackingAsync(id);
    }

   
}
