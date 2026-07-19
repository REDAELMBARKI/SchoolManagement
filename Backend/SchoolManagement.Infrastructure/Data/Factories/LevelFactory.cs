using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class LevelFactory : Factory<Level>
{
    private int _order = 1;

    public LevelFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Level> Make()
    {
        var branches = Context.Branches.Select(b => b.Id).ToList();

        return Task.FromResult(Level.Create(
            name: faker.PickRandom("A0", "A1", "A2", "B1", "B2", "C1", "C2"),
            branchId: faker.PickRandom(branches),
            order: _order++
        ));
    }
}