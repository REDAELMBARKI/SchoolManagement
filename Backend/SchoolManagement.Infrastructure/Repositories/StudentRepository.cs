using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Infrastructure.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(AppDbContext context) : base(context)
    {
    }

    protected override IQueryable<Student> Query()
    {
        return Context.Users.OfType<Student>()
            .Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null);
    }

    protected async Task<Student?> GetByIdForUpdateAsync(int id)
    {
        return await Query().FirstOrDefaultAsync(s => s.Id == new Guid(id.ToString()));
    }

    public async Task<Student> AddAsync(Student student)
    {
        return await base.AddAsync(student);
    }

    public async Task DeleteAsync(int id)
    {
        await base.DeleteAsync(new Guid(id.ToString()));
    }

    public async Task UpdateAsync(int id, Student student)
    {
        var dbStudent = await GetByIdForUpdateAsync(id);
        if (dbStudent is null) throw new NotFoundException($"Student with id {id} not found");
        student.Id = dbStudent.Id;
        Context.Entry(dbStudent).CurrentValues.SetValues(student);
        await Context.SaveChangesAsync();
    }
}
