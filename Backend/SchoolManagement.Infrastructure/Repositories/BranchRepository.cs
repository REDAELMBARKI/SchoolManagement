using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
namespace SchoolManagement.Infrastructure.Repositories;

public class BranchRepository : Repository<Branch> , IBranchRepository
{
    public BranchRepository(AppDbContext context) : base(context) { }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public new async Task<bool> ExistsAsync(int id)
    {
        return await Context.Branches.AnyAsync();
    }

    public async Task<List<Branch>> GetAllAsync()
    {
        return new List<Branch>();
    }

    public Task<Branch?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Branch?> GetOneAsync(int id)
    {
        return await Query().FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<Branch?> UpdateAsync(int id, Branch entity)
    {
        throw new NotImplementedException();
    }

    Task<Branch> IRepository<Branch>.AddAsync(Branch entity)
    {
        return AddAsync(entity);
    }
}