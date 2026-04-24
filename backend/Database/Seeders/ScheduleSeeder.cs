using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Seeders;

public class ScheduleSeeder : Seeder
{
    private readonly ScheduleFactory _factory;

    public ScheduleSeeder(AppDbContext context) : base(context)
    {
        _factory = new ScheduleFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await Context.Schedules.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}