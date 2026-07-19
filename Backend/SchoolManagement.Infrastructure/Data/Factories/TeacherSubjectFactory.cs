using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class TeacherSubjectFactory : Factory<TeacherSubject>
{
    public TeacherSubjectFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<TeacherSubject> Make()
    {
        var teachers = await Context.Teachers.Select(t => t.Id).ToListAsync();
        var subjects = await Context.Subjects.Select(s => s.Id).ToListAsync();

        return TeacherSubject.Create(
            teacherId: teachers.Any() ? faker.PickRandom(teachers) : Guid.Empty,
            subjectId: subjects.Any() ? faker.PickRandom(subjects) : Guid.Empty
        );
    }
}
