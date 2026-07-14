using Bogus;
using SchoolManagement.Infrastructure.Data ;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Domain.Utils;

namespace SchoolManagement.Infrastructure.Data.Factories ; 
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