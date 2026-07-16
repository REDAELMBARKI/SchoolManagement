using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Interfaces.Queries.Common;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public abstract class QueryServiceBase<T> : IQuery<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;

    protected QueryServiceBase(AppDbContext context)
    {
        Context = context;
    }

    protected virtual IQueryable<T> Query()
    {
        var query = Context.Set<T>().AsNoTracking().AsQueryable();
        query = query.Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null);
        return query;
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await Query().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await Context.Set<T>().AnyAsync(e => e.Id == id);
    }
}
