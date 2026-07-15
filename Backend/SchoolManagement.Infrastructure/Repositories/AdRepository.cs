using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
namespace SchoolManagement.Infrastructure.Repositories;

public class AdRepository : Repository<Ad> , IAdRepository
{
    public AdRepository(AppDbContext context) : base(context) { }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public new async Task<bool> ExistsAsync(int id)
    {
        return false;
    }

    public async Task<List<Ad>> GetAllAsync()
    {
        return new List<Ad>();
    }

    public async Task<Ad> GetByIdAsync(int id)
    {
        return new Ad();
    }

    public Task<Ad?> UpdateAsync(int id, Ad entity)
    {
        throw new NotImplementedException();
    }

    Task<Ad> IRepository<Ad>.AddAsync(Ad entity)
    {
        return AddAsync(entity);
    }
}