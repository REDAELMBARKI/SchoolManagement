using SchoolManagement.Backend.Database.Factories;
namespace SchoolManagement.Backend.Database.Seeders;

public class RoomSeeder : Seeder
{
    private readonly RoomFactory _factory;

    public RoomSeeder(AppDbContext context) : base(context)
    {
        _factory = new RoomFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await Context.Rooms.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}