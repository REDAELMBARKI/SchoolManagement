using SchoolManagement.Domain.Entities;
 using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Infrastructure.Data ;
namespace SchoolManagement.Infrastructure.Data.Factories;

public class BranchFactory : Factory<Branch>
{
    private readonly BranchRepository _repository;
    public BranchFactory(AppDbContext context, BranchRepository repository) : base(context)
    {
        _repository = repository;
    }

    protected override async Task<Branch> Make()
    {
        var companyName = faker.Company.CompanyName() ;
        var creationDate = DateTime.Now ;
        return new Branch
        {
            Name = companyName ,
            Slug = this.GenerateSlug(companyName),
            Phone = faker.Phone.PhoneNumber() ,
            Address = faker.Address.FullAddress() ,
            City = faker.Address.City() ,
            CreatedAt = creationDate ,
            UpdatedAt = creationDate
            
        };
    }
}