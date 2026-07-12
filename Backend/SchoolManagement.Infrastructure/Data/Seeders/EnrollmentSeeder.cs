using SchoolManagement.Backend.Data.Factories;
namespace SchoolManagement.Backend.Data.Seeders;
using SchoolManagement.Backend.Data ;
public class EnrollmentSeeder : Seeder
{
    private readonly EnrollmentFactory _factory;

    public EnrollmentSeeder(AppDbContext context) : base(context) 
    {
        _factory = new EnrollmentFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = await _factory.MakeMany(10);
        await Context.Enrollments.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}