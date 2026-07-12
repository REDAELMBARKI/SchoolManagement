using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data;
using SchoolManagement.Infrastructure.Interfaces;
namespace SchoolManagement.Backend.Repositories;

public class AdRepository : Repository<Ad> , IAdRepository
{
    public AdRepository(AppDbContext context) : base(context) { }
    
    public new async Task<bool> ExistsAsync(int id)
    {
        return false;
    }

    public async Task<List<Ad>> GetAllAsync()
    {
        return new List<Ad>();
    }

    public async Task<Ad> GetOneAsync(int id)
    {
        return new Ad();
    }

    
    
}