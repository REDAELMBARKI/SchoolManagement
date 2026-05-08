using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Exceptions;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

public class EnrollmentRepository : Repository<Enrollment>
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

    // TODO: Add business logic methods for you to implement:
    // - Check if student already enrolled in this subject
    // - Validate group capacity before enrollment
    // - Calculate fees based on Plan
    // - Get enrollments by student
    // - Get enrollments by group
    // - Process payment for enrollment
}
