
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;

namespace  SchoolManagement.Backend.Database.Seeders ; 

public class SessionSeeder : Seeder
{
    private readonly   SessionFactory _factory;

    public SessionSeeder(AppDbContext context) : base(context)
    {
            _factory = new   SessionFactory(context);

    } 

    public override async Task RunAsync()
    {
       List<Session> Sessions = _factory.MakeMany(10);
       await Context.Sessions.AddRangeAsync(Sessions);
       await Context.SaveChangesAsync() ;
    }
}