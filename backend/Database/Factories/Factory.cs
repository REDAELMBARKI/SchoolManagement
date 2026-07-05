using Bogus;
using Microsoft.VisualBasic;
using SchoolManagement.Backend.Contexts ;


namespace SchoolManagement.Backend.Database.Factories ; 
public abstract class Factory<T> where T : class 
{  
    protected AppDbContext _context;
    public Factory(AppDbContext context)
    {
        _context = context ;
    }
    protected readonly Faker faker = new Faker() ; 
    protected abstract  Task<T> Make() ; 

    public async  Task<List<T>> MakeMany(int count)
    {
        var tasks   = Enumerable.Range(0 , count)
               .Select(_ =>  Make()) 
               .ToList() ;
        var results  = await Task.WhenAll(tasks);
        return results.ToList();
    } 

    protected AppDbContext Context => _context ;
     
}