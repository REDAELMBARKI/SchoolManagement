using SchoolManagement.Backend.Entities;
using Slugify;
using SchoolManagement.Backend.Data ;

namespace SchoolManagement.Backend.Data.Factories;

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
        var ad =  new Ad
        {
            Name = name,
            BranchId = faker.PickRandom(branches) ,
            Slug = new SlugHelper().GenerateSlug(name),
            PlatformId = faker.PickRandom(platforms) 
        };

        return Task.FromResult(ad);
    }
}