using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Interfaces;

namespace SchoolManagement.Backend.Repositories;

public abstract class Repository<T> where T : class
{
    protected readonly AppDbContext _context;

    protected Repository(AppDbContext context)
    {
        _context = context;
    }

    protected virtual IQueryable<T> QueryWithTracking()
    {
        return _context.Set<T>().AsQueryable();
    }

    protected virtual IQueryable<T> Query()
    {
        return _context.Set<T>().AsNoTracking().AsQueryable();
    }


    protected async Task<T> AddAsync(T entity)
    {
        var entry = await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }
    
    protected AppDbContext Context => _context;
}
