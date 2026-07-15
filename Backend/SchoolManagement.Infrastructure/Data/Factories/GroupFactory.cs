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

        var group = new Group
        {
                Name       = faker.Lorem.Letter(1).ToUpper() + faker.Random.Int(1, 9),  // e.g. "A3", "B7"
                Capacity   = faker.Random.Int(10, 60),
                BranchId = faker.PickRandom(branches) ,

                Period     = faker.PickRandom("Morning", "Afternoon", "Evening", "Weekend"),
                LevelId    = faker.Random.Int(1, 5),
        } ; 
        return Task.FromResult(group);
    }
}