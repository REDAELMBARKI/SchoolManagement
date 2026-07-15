using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
namespace SchoolManagement.Infrastructure.Repositories;

public class CommercialAgentRepository : Repository<CommercialAgent>, ICommercialAgentRepository
{
    public CommercialAgentRepository(AppDbContext context) : base(context) { }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsAsync()
    {
        return false;
    }

    public async Task<List<CommercialAgent>> GetAllAsync()
    {
        return new List<CommercialAgent>();
    }

    public Task<CommercialAgent?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<CommercialAgent?> GetOneAsync(int id)
    {
        return new CommercialAgent();
    }

    public Task<CommercialAgent?> UpdateAsync(int id, CommercialAgent entity)
    {
        throw new NotImplementedException();
    }

    Task<CommercialAgent> IRepository<CommercialAgent>.AddAsync(CommercialAgent entity)
    {
        return AddAsync(entity);
    }
}