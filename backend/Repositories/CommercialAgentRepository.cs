using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

public class CommercialAgentRepository : Repository<CommercialAgent>
{
    public CommercialAgentRepository(AppDbContext context) : base(context) { }
    
    public async Task<bool> ExistsAsync()
    {
        return false;
    }

    public async Task<List<CommercialAgent>> GetAllAsync()
    {
        return new List<CommercialAgent>();
    }

    public async Task<CommercialAgent> GetOneAsync(int id)
    {
        return new CommercialAgent();
    }

}