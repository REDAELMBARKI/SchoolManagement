using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Interfaces;
namespace SchoolManagement.Infrastructure.Repositories;

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