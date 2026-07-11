using SchoolManagement.Backend.Data.Factories;
using SchoolManagement.Backend.Data ;
namespace SchoolManagement.Backend.Data.Seeders;

public class PlatformSeeder : Seeder
{
    private readonly PlatformFactory _factory;
    public PlatformSeeder(AppDbContext context) : base(context)
    {
        _factory = new PlatformFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = await _factory.MakeMany(5);
        await Context.Platforms.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}