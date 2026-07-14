using SchoolManagement.Infrastructure.Data.Factories;
namespace SchoolManagement.Infrastructure.Data.Seeders;
using SchoolManagement.Infrastructure.Data ;
public class BranchSeeder : Seeder
{
    private readonly AppDbContext _context;
    private readonly BranchFactory _factory ;
    public BranchSeeder(AppDbContext context , BranchFactory factory) : base(context)
    {
        _factory = factory;
    }

    public override async Task RunAsync()
    {
        var items = await _factory.MakeMany(10);
        await Context.Branches.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}