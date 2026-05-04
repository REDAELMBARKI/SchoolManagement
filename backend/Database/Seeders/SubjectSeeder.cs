using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Seeders ; 
public class SubjectSeeder : Seeder
{
    private readonly SubjectFactory _factory;

    public SubjectSeeder(AppDbContext context) : base(context)
    {
            _factory = new SubjectFactory(context);

    }
    
    public override async Task RunAsync()
    {
        List<Subject> subjects = _factory.MakeMany(4) ; 
        await Context.Subjects.AddRangeAsync(subjects) ; 
        await Context.SaveChangesAsync() ;
    }
}