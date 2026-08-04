using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Common.Repositories;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(AppDbContext context) : base(context)
    {
    }
    
 
}
