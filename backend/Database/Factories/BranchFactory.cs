using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Utils;
using Slugify;
namespace SchoolManagement.Backend.Database.Factories;

public class BranchFactory : Factory<Branch>
{
    public BranchFactory(AppDbContext context) : base(context)
    {
    }

    protected override Branch Make()
    {
        var companyName = faker.Company.CompanyName() ;
        var creationDate = DateTime.Now ;
        return new Branch
        {
            Name = companyName ,
            Slug = CustomSluger.Slug(companyName) ,
            Phone = faker.Phone.PhoneNumber() ,
            Address = faker.Address.FullAddress() ,
            City = faker.Address.City() ,
            CreatedAt = creationDate ,
            UpdatedAt = creationDate
            
        };
    }
}