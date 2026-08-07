using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Repositories;

public class GradeRepository : Repository<Grade>, IGradeRepository
{
    public GradeRepository(AppDbContext context) : base(context)
    {
    }
}
