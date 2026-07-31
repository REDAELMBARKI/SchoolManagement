using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(AppDbContext context) : base(context)
    {
    }
    
 
}
