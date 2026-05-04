using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Interfaces.Repos;
using SchoolManagement.Backend.Models;

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