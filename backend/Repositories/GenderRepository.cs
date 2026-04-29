using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Interfaces;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

public class GenderRepository : Repository<Gender> , IGenderRepository
{
    public GenderRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsAsync()
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