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
        var branches = Context.Branches.Select(b => b.Id).ToList();

         return new SchoolProgram
         {
            Name = name  ,
            BranchId = faker.PickRandom(branches) ,

            Slug = CustomSluger.Slug(name)
         } ;

     }
}