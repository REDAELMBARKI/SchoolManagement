using SchoolManagement.Infrastructure.Data.Factories;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data ;
using SchoolManagement.Infrastructure.Academic.Repositories;
using SchoolManagement.Infrastructure.Core.Repositories;
using SchoolManagement.Infrastructure.Common.Repositories;
namespace SchoolManagement.Infrastructure.Data.Seeders ; 
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