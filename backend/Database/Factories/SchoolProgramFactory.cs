using Bogus;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Utils;

namespace SchoolManagement.Backend.Database.Factories ; 
public class SchoolProgramFactory : Factory<SchoolProgram>
{
    public SchoolProgramFactory(AppDbContext context) : base(context)
    {
    }

    protected override SchoolProgram Make(){
         string name  = faker.PickRandom("English", "French", "Spanish", "Arabic", "German", "Italian") ;
         return new SchoolProgram
         {
            Name = name  ,
            Slug = CustomSluger.Slug(name)
         } ;

     }
}