using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Factories ; 
public class GroupFactory : Factory<Group>
{
    public GroupFactory(AppDbContext context) : base(context)
    {
    }

    protected override Group Make()
    {
        return new Group
        {
                Name       = faker.Lorem.Letter(1).ToUpper() + faker.Random.Int(1, 9),  // e.g. "A3", "B7"
                Capacity   = faker.Random.Int(10, 60),
                Period     = faker.PickRandom("Morning", "Afternoon", "Evening", "Weekend"),
                LevelId    = faker.Random.Int(1, 5),
                LanguageId = faker.Random.Int(1, 4),
                SessionId  = faker.Random.Int(1, 3),
        } ; 
    }
}