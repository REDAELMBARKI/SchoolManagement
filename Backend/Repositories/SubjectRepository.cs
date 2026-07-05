using SchoolManagement.Backend.Contexts;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories
{
    public class SubjectRepository : Repository<Subject>
    {
        public SubjectRepository(AppDbContext context) : base(context)
        {
        }
    }
}
