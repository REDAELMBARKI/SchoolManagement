using SchoolManagement.Infrastructure.Data.Factories;
using SchoolManagement.Infrastructure.Data ;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Seeders;

public class ScheduleSeeder : Seeder
{
    private readonly ScheduleFactory _factory;

    public ScheduleSeeder(AppDbContext context) : base(context)
    {
        _factory = new ScheduleFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = await _factory.MakeMany(10);
        await Context.Schedules.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}