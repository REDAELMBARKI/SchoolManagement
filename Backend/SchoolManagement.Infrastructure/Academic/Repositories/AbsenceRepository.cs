using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Repositories;

public class AbsenceRepository : Repository<Absence>, IAbsenceRepository
{
    public AbsenceRepository(AppDbContext context) : base(context)
    {
    }
}
