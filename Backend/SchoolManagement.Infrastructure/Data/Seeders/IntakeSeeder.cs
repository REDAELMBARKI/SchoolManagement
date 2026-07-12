using SchoolManagement.Backend.Data.Factories;
namespace SchoolManagement.Backend.Data.Seeders;
using SchoolManagement.Backend.Data ;
using SchoolManagement.Backend.Repositories;

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