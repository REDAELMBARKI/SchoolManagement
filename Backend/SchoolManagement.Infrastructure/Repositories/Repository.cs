using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Interfaces;
using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;
namespace SchoolManagement.Backend.Repositories;

public abstract class Repository<T> where T : BaseEntity
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
        var query =  _context.Set<T>().AsNoTracking().AsQueryable();
        
        // Add soft delete filter for Person entities (TPC hierarchy)
        if (typeof(Person).IsAssignableFrom(typeof(T)))
        {
            query = query.Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null);
        }
        
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
        bool hasColumn = Context.Model.FindEntityType(typeof(T))
                            .GetProperties()
                            .Any(p => p.Name == "Slug");
        if(!hasColumn)
            throw new InvalidOperationException($"The entity '{typeof(T).Name}' does not have a 'Slug' column/property.");

        return await this.Query().AnyAsync(e => EF.Property<string>(e, "Slug") == slug);
    }


    protected AppDbContext Context => _context;
}
