using Microsoft.EntityFrameworkCore;
using SchoolManagement.Interfaces;

namespace SchoolManagement.Repositories ;

public abstract class Repository<T>(AppDbContext _context) where T : class 
{
    protected IQueryable<T>  QueryWithTracking()
    {
        return _context.Set<T>().AsQueryable() ;
    }

    protected  IQueryable<T> Query()
    {
        return _context.Set<T>().AsNoTracking().AsQueryable() ;
    }
 
    
    protected AppDbContext Context => _context ;
}
