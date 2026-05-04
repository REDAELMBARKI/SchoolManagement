using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Interfaces.Repos;

using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

public class BranchRepository : Repository<Branch> , IBranchRepository
{
    public BranchRepository(AppDbContext context) : base(context) { }
    

    public new async Task<bool> ExistsAsync(int id)
    {
        return await Context.Branches.AnyAsync();
    }

    public async Task<List<Branch>> GetAllAsync()
    {
        return new List<Branch>();
    }

    public async Task<Branch?> GetOneAsync(int id)
    {
        return await Query().FirstOrDefaultAsync(b => b.Id == id);
    }

    
}