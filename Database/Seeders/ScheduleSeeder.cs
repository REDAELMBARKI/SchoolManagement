using SchoolManagement.Database.Factories;
using SchoolManagement.Models;

namespace SchoolManagement.Database.Seeders;

public class ScheduleSeeder : Seeder
{
    private readonly ScheduleFactory _factory;
    private readonly AppDbContext _context;

    public ScheduleSeeder(AppDbContext context)
    {
        _factory = new ScheduleFactory();
        _context = context;
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await _context.Schedules.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }
}