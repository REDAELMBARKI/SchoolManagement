using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Infrastructure.Repositories;

public class IntakeRepository : Repository<Intake>, IIntakeRepository
{
    public IntakeRepository(AppDbContext context) : base(context)
    {
    }
}
