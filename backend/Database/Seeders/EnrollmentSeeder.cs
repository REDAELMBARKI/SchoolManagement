using SchoolManagement.Backend.Database.Factories;
namespace SchoolManagement.Backend.Database.Seeders;

public class EnrollmentSeeder : Seeder
{
    private readonly EnrollmentFactory _factory;

    public EnrollmentSeeder(AppDbContext context) : base(context) 
    {
        _factory = new EnrollmentFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = _factory.MakeMany(10);
        await Context.Enrollments.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}