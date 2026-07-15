using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
namespace SchoolManagement.Infrastructure.Repositories;

public class EnrollmentRepository : Repository<Enrollment>  , IEnrollmentRepository
{
    public EnrollmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Enrollment>> GetAllAsync()
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

    public async Task<Enrollment?> GetOneAsync(int id)
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

    public async Task<Enrollment> AddAsync(Enrollment enrollment)
    {
        await Context.AddAsync(enrollment);
        await Context.SaveChangesAsync();
        return enrollment;
    }

    public async Task UpdateAsync(int id, Enrollment enrollment)
    {
        var dbEnrollment = await Context.Enrollments.FindAsync(id);
        if (dbEnrollment is null) throw new NotFoundException($"Enrollment with id {id} not found");
        enrollment.Id = dbEnrollment.Id;
        Context.Entry(dbEnrollment).CurrentValues.SetValues(enrollment);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var enrollment = await Context.Enrollments.FindAsync(id);
        if (enrollment is null) throw new NotFoundException($"Enrollment with id {id} not found");
        enrollment.DeletedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync();
    }

    Task<Enrollment?> IRepository<Enrollment>.UpdateAsync(int id, Enrollment entity)
    {
        throw new NotImplementedException();
    }

    public Task<Enrollment?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }


}
