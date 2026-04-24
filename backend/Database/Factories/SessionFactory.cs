using SchoolManagement.Backend.Models;
using System;

namespace SchoolManagement.Backend.Database.Factories ; 

public class SessionFactory : Factory<Session>
{
    public SessionFactory(AppDbContext context) : base(context)
    {
    }

    protected override Session Make()
    {
        var start = faker.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now.AddMonths(2));
        var durationMonths = faker.Random.Int(3, 12);
        var end = start.AddMonths(durationMonths);
        var season = faker.PickRandom("Spring", "Summer", "Fall", "Winter");
        var label = $"{start:yyyy}/{end:yyyy} {season}";

        return new Session
        {
            Label = label,
            StartDate = start,
            EndDate = end,
            IsCurrent = start <= DateTime.Now && end >= DateTime.Now
        };
    }
}