using SchoolManagement.Infrastructure.Data.Factories;
using SchoolManagement.Infrastructure.Data ;
namespace SchoolManagement.Infrastructure.Data.Seeders;

public class RoomSeeder : Seeder
{
    private readonly RoomFactory _factory;

    public RoomSeeder(AppDbContext context) : base(context)
    {
        _factory = new RoomFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = await _factory.MakeMany(10);
        await Context.Rooms.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}