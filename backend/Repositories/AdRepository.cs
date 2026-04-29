using SchoolManagement.Backend.Interfaces;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

public class AdRepository : Repository<Ad> , IAdRepository
{
    public AdRepository(AppDbContext context) : base(context) { }
    
    public async Task<bool> ExistsAsync()
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