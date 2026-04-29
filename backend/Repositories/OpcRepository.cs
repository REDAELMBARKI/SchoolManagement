using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Interfaces;

namespace SchoolManagement.Backend.Repositories;

public class OpcRepository : Repository<Opc> , IOpcRepository
{
    public OpcRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsAsync()
    {
        return false;
    }

    public async Task<List<Opc>> GetAllAsync()
    {
        return new List<Opc>();
    }

    public async Task<Opc> GetOneAsync(int id)
    {
        return new Opc();
    }

}