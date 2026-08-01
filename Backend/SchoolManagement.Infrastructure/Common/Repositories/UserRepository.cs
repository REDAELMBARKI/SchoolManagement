using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data ;

namespace SchoolManagement.Infrastructure.Common.Repositories;

public class UserRepository : Repository<DomainUser> 
{
    public UserRepository(AppDbContext context) : base(context) { }

 

}