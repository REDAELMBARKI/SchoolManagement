using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class AbsenceFactory : Factory<Absence>
{
    public AbsenceFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Absence> Make()
    {
        var students = await Context.Students.Select(s => s.Id).ToListAsync();
        var schedules = await Context.Schedules.Select(s => s.Id).ToListAsync();
        var statuses = new[] { "Absent", "Late" };

        var isJustified = faker.Random.Bool();
        var reason = isJustified ? faker.Lorem.Sentence() : null;

        return Absence.Create(
            studentId: students.Any() ? faker.PickRandom(students) : Guid.Empty,
            scheduleId: schedules.Any() ? faker.PickRandom(schedules) : Guid.Empty,
            date: faker.Date.Past(),
            status: faker.PickRandom(statuses),
            isJustified: isJustified,
            reason: reason
        );
    }
}
