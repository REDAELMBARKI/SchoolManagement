using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class GroupTeacherFactory : Factory<GroupTeacher>
{
    public GroupTeacherFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<GroupTeacher> Make()
    {
        var teachers = await Context.Teachers.Select(t => t.Id).ToListAsync();
        var groups = await Context.Groups.Select(g => g.Id).ToListAsync();

        return GroupTeacher.Create(
            teacherId: teachers.Any() ? faker.PickRandom(teachers) : Guid.Empty,
            groupId: groups.Any() ? faker.PickRandom(groups) : Guid.Empty
        );
    }
}
