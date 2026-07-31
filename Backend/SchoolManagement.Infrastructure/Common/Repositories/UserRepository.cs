using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data ;

namespace SchoolManagement.Infrastructure.Common.Repositories;

public class UserRepository : Repository<DomainUser> 
{
    public UserRepository(AppDbContext context) : base(context) { }

 

}