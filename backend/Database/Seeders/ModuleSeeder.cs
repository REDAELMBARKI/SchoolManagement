using SchoolManagement.Backend.Database.Factories;
namespace SchoolManagement.Backend.Database.Seeders;

public class ModuleSeeder : Seeder
{
    private readonly ModuleFactory _factory;

    public ModuleSeeder(AppDbContext context) : base(context)
    {
        _factory = new ModuleFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await Context.Modules.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}