using SchoolManagement.Backend.Data.Factories;
using SchoolManagement.Backend.Data ;
namespace SchoolManagement.Backend.Data.Seeders;

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