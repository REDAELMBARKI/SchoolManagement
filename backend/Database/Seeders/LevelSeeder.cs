
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;
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
       List<Level> Levels = await _factory.MakeMany(10);
       await Context.Levels.AddRangeAsync(Levels);
       await Context.SaveChangesAsync() ;
    }
}