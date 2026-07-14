using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class SubjectRepository : Repository<Subject>
    {
        public SubjectRepository(AppDbContext context) : base(context)
        {
        }
    }
}
