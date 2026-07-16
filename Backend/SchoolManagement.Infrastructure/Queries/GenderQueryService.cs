using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class GenderQueryService : QueryServiceBase<Gender>, IGenderQueryService
{
    public GenderQueryService(AppDbContext context) : base(context)
    {
    }

    public override async Task<Gender?> GetByIdAsync(int id)
    {
        return (await Query().FirstOrDefaultAsync(g => g.Id == id))!;
    }
}
