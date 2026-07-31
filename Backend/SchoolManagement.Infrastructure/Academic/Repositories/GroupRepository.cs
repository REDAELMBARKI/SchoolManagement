using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Repositories;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    public GroupRepository(AppDbContext context) : base(context)
    {
    }

}
