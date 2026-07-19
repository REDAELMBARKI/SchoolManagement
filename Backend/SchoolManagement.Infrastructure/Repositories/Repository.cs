using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
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

    public virtual async Task<T> AddAsync(T entity)
    {
        var entry = await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    { 
        var existing = await GetByIdAsync(entity.Id);
        if (existing == null)
            throw new NotFoundException($"No {typeof(T).Name} found with id {entity.Id}");
        _context.Set<T>()
         .Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Entry(entity).Property("DeletedAt").CurrentValue = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == id);
    }

    protected AppDbContext Context => _context;
}
