using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Infrastructure.Repositories;

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Enrollment> AddAsync(Enrollment enrollment)
    {
        return await base.AddAsync(enrollment);
    }

    public async Task UpdateAsync(Guid id, Enrollment enrollment)
    {
        var dbEnrollment = await GetByIdForUpdateAsync(id);
        if (dbEnrollment is null) throw new NotFoundException($"Enrollment with id {id} not found");
        enrollment.Id = dbEnrollment.Id;
        Context.Entry(dbEnrollment).CurrentValues.SetValues(enrollment);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await base.DeleteAsync(id);
    }
}
