using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Seeders ; 
public class SchoolProgramSeeder : Seeder
{
    private readonly SchoolProgramFactory _factory;

    public SchoolProgramSeeder(AppDbContext context) : base(context)
    {
            _factory = new SchoolProgramFactory(context);

    }
    
    public override async Task RunAsync()
    {
        List<SchoolProgram> languages = _factory.MakeMany(4) ; 
        await Context.SchoolPrograms.AddRangeAsync(languages) ; 
        await Context.SaveChangesAsync() ;
    }
}