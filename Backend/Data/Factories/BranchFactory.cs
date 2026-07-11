using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Repositories;
using SchoolManagement.Backend.Utils;

using SchoolManagement.Backend.Data ;

namespace SchoolManagement.Backend.Data.Factories;

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