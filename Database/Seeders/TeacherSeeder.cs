using SchoolManagement.Database.Factories;
namespace SchoolManagement.Database.Seeders;

public class TeacherSeeder : Seeder
{
    private readonly TeacherFactory _factory;
    private readonly AppDbContext _context;

    public TeacherSeeder(AppDbContext context)
    {
        _factory = new TeacherFactory();
        _context = context;
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await _context.Teachers.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }
}