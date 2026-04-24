using SchoolManagement.Backend.Database.Factories;
namespace SchoolManagement.Backend.Database.Seeders;

public class IntakeSeeder : Seeder
{
    private readonly IntakeFactory _factory;

    public IntakeSeeder(AppDbContext context) : base(context)
    {
        _factory = new IntakeFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await Context.Intakes.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}