using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Contexts ;
namespace SchoolManagement.Backend.Database.Seeders;

public class GradeSeeder : Seeder
{
    private readonly GradeFactory _factory;

    public GradeSeeder(AppDbContext context) : base(context)
    {
        _factory = new GradeFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = await _factory.MakeMany(10);
        await Context.Grades.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}