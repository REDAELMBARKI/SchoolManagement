using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Seeders;

public class DaySeeder : Seeder
{
    public DaySeeder(AppDbContext context) : base(context)
    {
    }

    public override async Task RunAsync()
    {
        if (await Context.Days.AnyAsync()) return;

        var dayNames = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        var days = dayNames.Select(name => Day.Create(name)).ToList();

        await Context.Days.AddRangeAsync(days);
        await Context.SaveChangesAsync();
    }
}
