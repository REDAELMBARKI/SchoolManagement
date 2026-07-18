using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public abstract class Repository<T> : IRepository<T> where T : AggregateRoot
{
    protected readonly AppDbContext _context;

    protected Repository(AppDbContext context)
    {
        _context = context;
    }

    protected virtual IQueryable<T> Query()
    {
        var query = _context.Set<T>().AsQueryable();
        query = query.Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null);
        return query;
    }

    public async Task<T> AddAsync(T entity)
    {
        var entry = await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await Query().FirstOrDefaultAsync(e => e.Id == id);
        if (entity != null)
        {
            _context.Entry(entity).Property("DeletedAt").CurrentValue = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<T?> FindByIdAsync(int id)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == new Guid(id.ToString()));
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await Query().ToListAsync();
    }

    protected virtual async Task<T?> GetByIdForUpdateAsync(Guid id)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == id);
    }

    protected AppDbContext Context => _context;
}
