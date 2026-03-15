using SchoolManagement.Database.Factories;
namespace SchoolManagement.Database.Seeders;

public class GradeSeeder : Seeder
{
    private readonly GradeFactory _factory;
    private readonly AppDbContext _context;

    public GradeSeeder(AppDbContext context)
    {
        _factory = new GradeFactory();
        _context = context;
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await _context.Grades.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }
}