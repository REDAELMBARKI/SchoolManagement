using Bogus;
using Microsoft.VisualBasic;
using SchoolManagement.Infrastructure.Data ; 
namespace SchoolManagement.Infrastructure.Data.Factories; 
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
        var results = new List<T>();
        for (int i = 0; i < count; i++)
        {
            var item = await Make();
            results.Add(item);
        }
        return results;
    } 


    protected string GenerateSlug(params string[] values)
    {
        var slug = string.Join("-", values.Select(v => v.ToLowerInvariant().Trim()));
        return slug;
    }

    protected AppDbContext Context => _context ;
     
}