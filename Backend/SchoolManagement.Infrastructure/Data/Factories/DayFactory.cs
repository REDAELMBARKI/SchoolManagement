using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class DayFactory : Factory<Day>
{
    public DayFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Day> Make()
    {
        var dayNames = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        return Task.FromResult(Day.Create(faker.PickRandom(dayNames)));
    }
}
