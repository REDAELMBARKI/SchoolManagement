
using SchoolManagement.Database.Factories;
using SchoolManagement.Models;

namespace  SchoolManagement.Database.Seeders ; 

public class LevelSeeder : Seeder
{
    private readonly   LevelFactory _factory;
    private readonly AppDbContext _context;

    public LevelSeeder(AppDbContext context)
    {
            _factory = new   LevelFactory();
            _context = context ;

    } 

    public override async Task RunAsync()
    {
       List<Level> Levels = _factory.MakeMany(10);
       await _context.Levels.AddRangeAsync(Levels);
       await _context.SaveChangesAsync() ;
    }
}