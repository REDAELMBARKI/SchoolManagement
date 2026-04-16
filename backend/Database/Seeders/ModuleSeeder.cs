using SchoolManagement.Database.Factories;
namespace SchoolManagement.Database.Seeders;

public class ModuleSeeder : Seeder
{
    private readonly ModuleFactory _factory;
    private readonly AppDbContext _context;

    public ModuleSeeder(AppDbContext context)
    {
        _factory = new ModuleFactory();
        _context = context;
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await _context.Modules.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }
}