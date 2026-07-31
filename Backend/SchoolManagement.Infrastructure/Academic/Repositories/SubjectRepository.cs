using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Repositories;

public class SubjectRepository : Repository<Subject>, ISubjectRepository
{
    public SubjectRepository(AppDbContext context) : base(context)
    {
    }
}
