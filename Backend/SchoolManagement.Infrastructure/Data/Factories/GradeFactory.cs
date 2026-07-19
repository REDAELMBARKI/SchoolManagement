using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class GradeFactory : Factory<Grade>
{
    public GradeFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Grade> Make()
    {
        var students = await Context.Students.Select(s => s.Id).ToListAsync();
        var groupTeachers = await Context.GroupTeachers.Select(gt => gt.Id).ToListAsync();
        var branches = await Context.Branches.Select(b => b.Id).ToListAsync();

        var evaluationTypes = new[] { "Quiz", "Midterm", "Final", "Assignment" };
        var maxScore = faker.Random.Float(50, 100);
        var score = faker.Random.Float(0, maxScore);
        var evaluationDate = faker.Date.Past();

        return Grade.Create(
            evaluationType: faker.PickRandom(evaluationTypes),
            score: score,
            maxScore: maxScore,
            evaluationDate: evaluationDate,
            comment: faker.Random.Bool() ? faker.Lorem.Sentence() : null,
            studentId: students.Any() ? faker.PickRandom(students) : Guid.Empty,
            groupTeacherId: groupTeachers.Any() ? faker.PickRandom(groupTeachers) : Guid.Empty,
            branchId: branches.Any() ? faker.PickRandom(branches) : Guid.Empty
        );
    }
}