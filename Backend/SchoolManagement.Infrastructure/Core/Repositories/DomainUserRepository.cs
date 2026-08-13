using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class DomainUserRepository : Repository<DomainUser>, IDomainUserRepository
{
    public DomainUserRepository(AppDbContext context) : base(context)
    {
    }
}
