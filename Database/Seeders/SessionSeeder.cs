
using SchoolManagement.Database.Factories;
using SchoolManagement.Models;

namespace  SchoolManagement.Database.Seeders ; 

public class SessionSeeder : Seeder
{
    private readonly   SessionFactory _factory;
    private readonly AppDbContext _context;

    public SessionSeeder(AppDbContext context)
    {
            _factory = new   SessionFactory();
            _context = context ;

    } 

    public override async Task RunAsync()
    {
       List<Session> Sessions = _factory.MakeMany(10);
       await _context.Sessions.AddRangeAsync(Sessions);
       await _context.SaveChangesAsync() ;
    }
}