using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Interfaces;
namespace SchoolManagement.Infrastructure.Repositories;

public class OpcRepository : Repository<Opc> , IOpcRepository
{
    public OpcRepository(AppDbContext context) : base(context) { }

    public new async Task<bool> ExistsAsync(int id)
    {
        return false;
    }

    public async Task<List<Opc>> GetAllAsync()
    {
        return new List<Opc>();
    }

    public async Task<Opc?> GetOneAsync(int id)
    {
        return new Opc();
    }

}