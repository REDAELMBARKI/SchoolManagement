using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories ; 
public class GroupFactory : Factory<Group>
{
    public GroupFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Group> Make()
    {
        var branches = Context.Branches.Select(b => b.Id).ToList();
        var levels  = Context.Levels.Select(l => l.Id).ToList();
        var subjects = Context.Subjects.Select(s => s.Id).ToList();

        var group = Group.Create(
                name       : faker.Lorem.Letter(1).ToUpper() + faker.Random.Int(1, 9),  // e.g. "A3", "B7"
                capacity   : faker.Random.Int(10, 60),
                period     : faker.PickRandom("Morning", "Afternoon", "Evening", "Weekend"),
                branchId : faker.PickRandom(branches) ,
                levelId    : faker.PickRandom(levels) , 
                subjectId: faker.PickRandom(subjects)
        ); 
        return Task.FromResult(group);
    }
}