using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data ;

namespace SchoolManagement.Infrastructure.Repositories;

public class UserRepository : Repository<User> 
{
    public UserRepository(AppDbContext context) : base(context) { }

 

}