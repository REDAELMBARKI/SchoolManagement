using SchoolManagement.Backend.Database.Factories;
namespace SchoolManagement.Backend.Database.Seeders;
using SchoolManagement.Backend.Contexts ;
public class BranchSeeder : Seeder
{
    private readonly AppDbContext _context;
    private readonly BranchFactory _factory ;
    public BranchSeeder(AppDbContext context , BranchFactory _factory) : base(context)
    {
        _factory = _factory;
    }

    public override async Task RunAsync()
    {
        var items = await _factory.MakeMany(10);
        await Context.Branches.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}