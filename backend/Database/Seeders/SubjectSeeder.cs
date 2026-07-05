using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;
using SchoolManagement.Backend.Repositories;
namespace SchoolManagement.Backend.Database.Seeders ; 
public class SubjectSeeder : Seeder
{
    private readonly SubjectFactory _factory;

    public SubjectSeeder(AppDbContext context , SubjectRepository repo) : base(context)
    {
            _factory = new SubjectFactory(context , repo);

    }
    
    public override async Task RunAsync()
    {
        List<Subject> subjects = await _factory.MakeMany(4) ; 
        await Context.Subjects.AddRangeAsync(subjects) ; 
        await Context.SaveChangesAsync() ;
    }
}