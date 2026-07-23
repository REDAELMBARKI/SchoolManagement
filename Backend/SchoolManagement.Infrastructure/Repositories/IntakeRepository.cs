using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class IntakeRepository : Repository<Intake>, IIntakeRepository
{
    public IntakeRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<Intake?> GetByIdAsync(Guid id)
    {
        return await Query()
            .Include(i => i.Students)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Intake?> GetIntakeByStudentId(Guid studentId)
    {
        return await Query()
                      .Where(i => i.Students.Any(s => s.Id == studentId))
                      .FirstOrDefaultAsync();
    }
}
