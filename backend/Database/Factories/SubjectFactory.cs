using Bogus;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;
using SchoolManagement.Backend.Repositories;
using SchoolManagement.Backend.Utils;

namespace SchoolManagement.Backend.Database.Factories ; 
public class SubjectFactory : Factory<Subject>
{
    private readonly SubjectRepository _repository;
    public SubjectFactory(AppDbContext context , SubjectRepository _repository) : base(context)
    {
    }

    protected override async Task<Subject> Make(){
         string name  = faker.PickRandom("English", "French", "Spanish", "Arabic", "German", "Italian") ;
         var branches = Context.Branches.Select(b => b.Id).ToList();

         return new Subject
         {
            Name = name  ,
            BranchId = faker.PickRandom(branches) ,
            Slug = await CustomSluger.Slug(slug => _repository.IsExistBySlug(slug), name)
             
         };

     }
}