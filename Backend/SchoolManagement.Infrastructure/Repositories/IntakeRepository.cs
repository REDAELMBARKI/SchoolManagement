using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;

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

    public async Task UpdateAsync(int id, Intake intake)
    {
        var dbIntake = await GetByIdForUpdateAsync(id);
        if (dbIntake is null) throw new NotFoundException($"No intake found with id {id}");
        intake.Id = dbIntake.Id;
        Context.Entry(dbIntake).CurrentValues.SetValues(intake);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await base.DeleteAsync(id);
    }
}
