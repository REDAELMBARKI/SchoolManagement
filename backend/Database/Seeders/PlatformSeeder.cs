using SchoolManagement.Backend.Database.Factories;
namespace SchoolManagement.Backend.Database.Seeders;

public class PlatformSeeder : Seeder
{
    private readonly PlatformFactory _factory;
    public PlatformSeeder(AppDbContext context) : base(context)
    {
        _factory = new PlatformFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(5);
        await Context.Platforms.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}