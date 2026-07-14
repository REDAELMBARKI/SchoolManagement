
using SchoolManagement.Infrastructure.Data.Factories;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data ;
namespace  SchoolManagement.Infrastructure.Data.Seeders ; 

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