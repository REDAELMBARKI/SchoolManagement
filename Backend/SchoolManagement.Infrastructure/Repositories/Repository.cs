using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Interfaces; 
using SchoolManagement.Domain.Entities;
 using SchoolManagement.Infrastructure.Data ;
using SchoolManagement.Domain.Common;
namespace SchoolManagement.Infrastructure.Repositories;

public abstract class Repository<T> where T : AggregateRoot
{
    protected readonly AppDbContext _context;

    protected Repository(AppDbContext context)
    {
        _context = context;
    }

    protected virtual IQueryable<T> QueryWithTracking()
    {
        var query = _context.Set<T>().AsQueryable();
        query = query.Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null);
        return query;
    }

    protected virtual IQueryable<T> Query()
    {
        var query =  _context.Set<T>().AsNoTracking().AsQueryable();
        query = query.Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null);
        
        return query;
    }


    protected async Task<T> AddAsync(T entity)
    {
        var entry = await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
       return await Context.Set<T>().AnyAsync(e => e.Id == id) ;
    }
     

    public  async Task<bool> IsExistBySlug(string slug)
    {
        bool hasColumn =    _context.Model.FindEntityType(typeof(T))
                            .GetProperties()
                            .Any(p => p.Name == "Slug");
        if(!hasColumn)
            throw new InvalidOperationException($"The entity '{typeof(T).Name}' does not have a 'Slug' column/property.");

        return await this.Query().AnyAsync(e => EF.Property<string>(e, "Slug") == slug);
    }


    protected AppDbContext Context => _context;
}
