using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Seeders;

public class TimeSlotSeeder : Seeder
{
    public TimeSlotSeeder(AppDbContext context) : base(context)
    {
    }

    public override async Task RunAsync()
    {
        if (await Context.TimeSlots.AnyAsync()) return;

        var timeSlots = new List<TimeSlot>
        {
            TimeSlot.Create(new TimeOnly(8, 0), new TimeOnly(9, 0)),
            TimeSlot.Create(new TimeOnly(9, 0), new TimeOnly(10, 0)),
            TimeSlot.Create(new TimeOnly(10, 0), new TimeOnly(11, 0)),
            TimeSlot.Create(new TimeOnly(11, 0), new TimeOnly(12, 0)),
            TimeSlot.Create(new TimeOnly(13, 0), new TimeOnly(14, 0)),
            TimeSlot.Create(new TimeOnly(14, 0), new TimeOnly(15, 0)),
            TimeSlot.Create(new TimeOnly(15, 0), new TimeOnly(16, 0)),
            TimeSlot.Create(new TimeOnly(16, 0), new TimeOnly(17, 0)),
        };

        await Context.TimeSlots.AddRangeAsync(timeSlots);
        await Context.SaveChangesAsync();
    }
}
