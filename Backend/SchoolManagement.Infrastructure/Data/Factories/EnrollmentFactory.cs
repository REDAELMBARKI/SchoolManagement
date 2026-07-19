using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class EnrollmentFactory : Factory<Enrollment>
{
    public EnrollmentFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Enrollment> Make()
    {
        var students = await Context.Students.Select(s => s.Id).ToListAsync();
        var subjects = await Context.Subjects.Select(s => s.Id).ToListAsync();
        var groups = await Context.Groups.Select(g => g.Id).ToListAsync();
        var branches = await Context.Branches.Select(b => b.Id).ToListAsync();
        var plans = await Context.Plans.Select(p => p.Id).ToListAsync();

        var statuses = new[] { "Active", "Dropped", "Completed" };

        return Enrollment.Create(
            studentId: students.Any() ? faker.PickRandom(students) : Guid.Empty,
            subjectId: subjects.Any() ? faker.PickRandom(subjects) : Guid.Empty,
            groupId: groups.Any() ? faker.PickRandom(groups) : Guid.Empty,
            branchId: branches.Any() ? faker.PickRandom(branches) : Guid.Empty,
            planId: plans.Any() ? faker.PickRandom(plans) : Guid.Empty,
            enrolledAt: faker.Date.Past(),
            status: faker.PickRandom(statuses),
            notes: faker.Random.Bool() ? faker.Lorem.Sentence() : null
        );
    }
}