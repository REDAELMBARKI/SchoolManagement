using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class SubjectQueryService : QueryServiceBase<Subject>, ISubjectQueryService
{
    public SubjectQueryService(AppDbContext context) : base(context)
    {
    }

    public Task<List<Subject>> GetAllAsync()
    {
        return Query().ToListAsync();
    }

    public Task<Subject?> GetByIdAsync(int id)
    {
        return Query().FirstOrDefaultAsync(s => s.Id == id);
    }
}
