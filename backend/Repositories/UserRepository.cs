using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

public class UserRepository : Repository<User>
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<List<int>> GetUsersIds()
    {
        return await Context.Users.Select(g => g.Id).ToListAsync(); 
    }

}