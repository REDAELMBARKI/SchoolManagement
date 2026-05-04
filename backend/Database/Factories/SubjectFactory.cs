using Bogus;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Utils;

namespace SchoolManagement.Backend.Database.Factories ; 
public class SubjectFactory : Factory<Subject>
{
    public SubjectFactory(AppDbContext context) : base(context)
    {
    }

    protected override Subject Make(){
         string name  = faker.PickRandom("English", "French", "Spanish", "Arabic", "German", "Italian") ;
        var branches = Context.Branches.Select(b => b.Id).ToList();

         return new Subject
         {
            Name = name  ,
            BranchId = faker.PickRandom(branches) ,

            Slug = CustomSluger.Slug(name)
         } ;

     }
}