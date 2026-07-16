using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class StudentQueryService : QueryServiceBase<Student>, IStudentQueryService
{
    public StudentQueryService(AppDbContext context) : base(context)
    {
    }

    protected override IQueryable<Student> Query()
    {
        return this.Context.Users.OfType<Student>()
            .Include(s => s.Gender)
            .Include(s => s.Parents)
            .Include(s => s.Intake)
            .Include(s => s.Enrollments)
            .AsNoTracking().AsQueryable()
            .Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null);
    }

    public override async Task<List<Student>> GetAllAsync()
    {
        return await this.Query().ToListAsync();
    }

    public async Task<Student> FindByIdAsync(int id)
    {
        var student = await this.Query().FirstOrDefaultAsync(s => s.Id == id);
        if (student is null) throw new NotFoundException($"Student with id {id} not found");
        return student;
    }

    public override async Task<Student?> GetByIdAsync(int id)
    {
        return await this.Query().FirstOrDefaultAsync(s => s.Id == id);
    }
}
