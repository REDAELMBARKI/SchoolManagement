using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class TimeSlotFactory : Factory<TimeSlot>
{
    public TimeSlotFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<TimeSlot> Make()
    {
        var startHour = faker.Random.Int(8, 17);
        var endHour = startHour + 1;
        return Task.FromResult(TimeSlot.Create(
            new TimeOnly(startHour, 0),
            new TimeOnly(endHour, 0)
        ));
    }
}
