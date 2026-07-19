using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class RoomFactory : Factory<Room>
{
    public RoomFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Room> Make()
    {
        var branches = Context.Branches.Select(b => b.Id).ToList();
        var floors = new[] { "Ground", "1st", "2nd", "3rd" };

        return Task.FromResult(Room.Create(
            name: faker.Commerce.ProductName(),
            capacity: faker.Random.Int(10, 50),
            floor: faker.PickRandom(floors),
            description: faker.Lorem.Sentence(),
            branchId: faker.PickRandom(branches)
        ));
    }
}