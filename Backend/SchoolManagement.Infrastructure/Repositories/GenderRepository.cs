using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities; 
using SchoolManagement.Infrastructure.Data; 
using SchoolManagement.Domain.Interfaces; 
namespace SchoolManagement.Infrastructure.Repositories;

public class GenderRepository : Repository<Gender> , IGenderRepository
{
    public GenderRepository(AppDbContext context) : base(context) { }

    public new async Task<bool> ExistsAsync(int id)
    {
        return false;
    }

    public async Task<List<Gender>> GetAllAsync()
    {
        return new List<Gender>();
    }

    public async Task<Gender> GetOneAsync(int id)
    {
        return new Gender();
    }

}