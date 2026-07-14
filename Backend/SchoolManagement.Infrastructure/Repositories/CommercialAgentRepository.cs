using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Interfaces;
namespace SchoolManagement.Infrastructure.Repositories;

public class CommercialAgentRepository : Repository<CommercialAgent>, ICommercialAgentRepository
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

    public async Task<CommercialAgent?> GetOneAsync(int id)
    {
        return new CommercialAgent();
    }

}