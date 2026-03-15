using SchoolManagement.Database.Factories;
using SchoolManagement.Models;

namespace SchoolManagement.Database.Seeders ; 
public class ParentSeeder : Seeder 
{
     
    private readonly ParentFactory _factory;
    private readonly AppDbContext _context;

    public ParentSeeder(AppDbContext context)
    {
            _factory = new ParentFactory();
            _context = context ;

    } 

    public override async Task RunAsync()
    {
        List<Parent> Parents  = _factory.MakeMany(10) ; 
        await _context.Parents.AddRangeAsync(Parents);
        await _context.SaveChangesAsync();
        
    }
}