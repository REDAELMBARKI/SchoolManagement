using SchoolManagement.Database.Factories;
namespace SchoolManagement.Database.Seeders;

public class EnrollmentSeeder : Seeder
{
    private readonly EnrollmentFactory _factory;
    private readonly AppDbContext _context;

    public EnrollmentSeeder(AppDbContext context)
    {
        _factory = new EnrollmentFactory();
        _context = context;
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await _context.Enrollments.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }
}