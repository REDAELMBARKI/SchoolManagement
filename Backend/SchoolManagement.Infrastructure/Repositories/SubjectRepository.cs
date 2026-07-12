using SchoolManagement.Backend.Data;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Backend.Repositories
{
    public class SubjectRepository : Repository<Subject>
    {
        public SubjectRepository(AppDbContext context) : base(context)
        {
        }
    }
}
