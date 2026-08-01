using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
 using SchoolManagement.Infrastructure.Academic.Repositories;
using SchoolManagement.Infrastructure.Core.Repositories;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data ;
using System.ComponentModel.DataAnnotations;
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
        return  Branch.Create(
            name : companyName ,
            slug : this.GenerateSlug(companyName),
            phone : faker.Phone.PhoneNumber() ,
            address : faker.Address.FullAddress() ,
            city : faker.Address.City()
        );
    }
}