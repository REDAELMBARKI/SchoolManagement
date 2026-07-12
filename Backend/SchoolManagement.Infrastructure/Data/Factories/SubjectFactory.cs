using Bogus;
using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;
using SchoolManagement.Backend.Repositories;
using SchoolManagement.Backend.Utils;

namespace SchoolManagement.Backend.Data.Factories ; 
public class SubjectFactory : Factory<Subject>
{
    private readonly SubjectRepository _repository;
    public SubjectFactory(AppDbContext context , SubjectRepository repository) : base(context)
    {
        _repository = repository;
    }

    protected override async Task<Subject> Make(){
         string name  = faker.PickRandom("English", "French", "Spanish", "Arabic", "German", "Italian") ;
         var branches = Context.Branches.Select(b => b.Id).ToList();

         return new Subject
         {
            Name = name  ,
            BranchId = faker.PickRandom(branches) ,
            Slug = this.GenerateSlug(name)
         };

     }
}