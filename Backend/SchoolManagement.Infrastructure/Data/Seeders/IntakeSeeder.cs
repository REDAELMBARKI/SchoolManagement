using SchoolManagement.Infrastructure.Data.Factories;
namespace SchoolManagement.Infrastructure.Data.Seeders;
using SchoolManagement.Infrastructure.Data ;
using SchoolManagement.Infrastructure.Repositories;

public class IntakeSeeder : Seeder
{
    private readonly IntakeFactory _factory;

    public IntakeSeeder(AppDbContext context , IntakeRepository _repo) : base(context)
    {
        _factory = new IntakeFactory(context  , _repo);
    }

    public override async Task RunAsync()
    {
        var items = await _factory.MakeMany(10);
        await Context.Intakes.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}