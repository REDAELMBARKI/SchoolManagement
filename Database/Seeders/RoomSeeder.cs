using SchoolManagement.Database.Factories;
namespace SchoolManagement.Database.Seeders;

public class RoomSeeder : Seeder
{
    private readonly RoomFactory _factory;
    private readonly AppDbContext _context;

    public RoomSeeder(AppDbContext context)
    {
        _factory = new RoomFactory();
        _context = context;
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await _context.Rooms.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }
}