using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities; 
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
namespace SchoolManagement.Infrastructure.Repositories;

public class GenderRepository : Repository<Gender> , IGenderRepository
{
    public GenderRepository(AppDbContext context) : base(context) { }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public new async Task<bool> ExistsAsync(int id)
    {
        return false;
    }

    public async Task<List<Gender>> GetAllAsync()
    {
        return new List<Gender>();
    }

    public Task<Gender?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Gender> GetOneAsync(int id)
    {
        return new Gender();
    }

    public Task<Gender?> UpdateAsync(int id, Gender entity)
    {
        throw new NotImplementedException();
    }

    Task<Gender> IRepository<Gender>.AddAsync(Gender entity)
    {
        return AddAsync(entity);
    }
}