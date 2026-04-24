using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Seeders ; 
public class LanguageSeeder : Seeder
{
    private readonly LanguageFactory _factory;

    public LanguageSeeder(AppDbContext context) : base(context)
    {
            _factory = new LanguageFactory(context);

    }
    
    public override async Task RunAsync()
    {
        List<Language> languages = _factory.MakeMany(4) ; 
        await Context.Languages.AddRangeAsync(languages) ; 
        await Context.SaveChangesAsync() ;
    }
}