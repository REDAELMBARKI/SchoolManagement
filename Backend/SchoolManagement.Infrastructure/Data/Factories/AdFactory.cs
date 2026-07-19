using SchoolManagement.Domain.Entities;
using Slugify;
using SchoolManagement.Infrastructure.Data ;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class AdFactory : Factory<Ad>
{
    public AdFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Ad> Make()
    {
        var name = faker.Commerce.ProductName();
        var platforms = Context.Platforms.Select(p => p.Id).ToList();
        var branches = Context.Branches.Select(b => b.Id).ToList();
        var ad = Ad.Create( name : name, 
                   slug : new SlugHelper().GenerateSlug(name), 
                   platformId : faker.PickRandom(platforms),
                   branchId : faker.PickRandom(branches));  
        return Task.FromResult(ad);
    }
}