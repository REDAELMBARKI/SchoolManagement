using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class ScheduleFactory : Factory<Schedule>
{
    public ScheduleFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Schedule> Make()
    {
        var branches = await Context.Branches.Select(b => b.Id).ToListAsync();
        var teachers = await Context.Teachers.Select(t => t.Id).ToListAsync();
        var rooms = await Context.Rooms.Select(r => r.Id).ToListAsync();
        var days = await Context.Days.Select(d => d.Id).ToListAsync();
        var timeSlots = await Context.TimeSlots.Select(ts => ts.Id).ToListAsync();
        var groups = await Context.Groups.Select(g => g.Id).ToListAsync();
        var subjects = await Context.Subjects.Select(s => s.Id).ToListAsync();

        return Schedule.Create(
            branchId: branches.Any() ? faker.PickRandom(branches) : Guid.Empty,
            teacherId: teachers.Any() ? faker.PickRandom(teachers) : Guid.Empty,
            roomId: rooms.Any() ? faker.PickRandom(rooms) : Guid.Empty,
            dayId: days.Any() ? faker.PickRandom(days) : Guid.Empty,
            timeSlotId: timeSlots.Any() ? faker.PickRandom(timeSlots) : Guid.Empty,
            groupId: groups.Any() ? faker.PickRandom(groups) : Guid.Empty,
            subjectId: subjects.Any() ? faker.PickRandom(subjects) : Guid.Empty
        );
    }
}