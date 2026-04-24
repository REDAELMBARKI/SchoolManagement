
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;

namespace  SchoolManagement.Backend.Database.Seeders ; 

public class LevelSeeder : Seeder
{
    private readonly   LevelFactory _factory;

    public LevelSeeder(AppDbContext context) : base(context)
    {
            _factory = new   LevelFactory(context);

    } 

    public override async Task RunAsync()
    {
       List<Level> Levels = _factory.MakeMany(10);
       await Context.Levels.AddRangeAsync(Levels);
       await Context.SaveChangesAsync() ;
    }
}