using SchoolManagement.Database.Factories;
using SchoolManagement.Models;

namespace SchoolManagement.Database.Seeders ; 
public class LanguageSeeder : Seeder
{
    private readonly LanguageFactory _factory;
    private readonly AppDbContext _context;

    public LanguageSeeder(AppDbContext context)
    {
            _factory = new LanguageFactory();
            _context = context ;

    }
    
    public override async Task RunAsync()
    {
        List<Language> languages = _factory.MakeMany(4) ; 
        await _context.Languages.AddRangeAsync(languages) ; 
        await _context.SaveChangesAsync() ;
    }
}