using SchoolManagement.Backend.Data.Factories;
using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;
using SchoolManagement.Backend.Repositories;
namespace SchoolManagement.Backend.Data.Seeders ; 
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