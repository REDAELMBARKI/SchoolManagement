using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
namespace SchoolManagement.Infrastructure.Repositories;

public class OpcRepository : Repository<Opc> , IOpcRepository
{
    public OpcRepository(AppDbContext context) : base(context) { }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public new async Task<bool> ExistsAsync(int id)
    {
        return false;
    }

    public async Task<List<Opc>> GetAllAsync()
    {
        return new List<Opc>();
    }

    public Task<Opc?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Opc?> GetOneAsync(int id)
    {
        return new Opc();
    }

    public Task<Opc?> UpdateAsync(int id, Opc entity)
    {
        throw new NotImplementedException();
    }

    Task<Opc> IRepository<Opc>.AddAsync(Opc entity)
    {
        return AddAsync(entity);
    }
}