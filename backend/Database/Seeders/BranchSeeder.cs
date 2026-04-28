using SchoolManagement.Backend.Database.Factories;
namespace SchoolManagement.Backend.Database.Seeders;

public class BranchSeeder : Seeder
{
    private readonly AppDbContext _context;
    private readonly BranchFactory _factory ;
    public BranchSeeder(AppDbContext context) : base(context)
    {
        _factory = new BranchFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await Context.Branches.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}