using Bogus;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Factories ; 
public class LanguageFactory : Factory<Language>
{
    public LanguageFactory(AppDbContext context) : base(context)
    {
    }

    protected override Language Make(){
         return new Language
         {
            Name = faker.PickRandom("English", "French", "Spanish", "Arabic", "German", "Italian") 
         } ;

     }
}