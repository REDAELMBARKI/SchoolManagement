using SchoolManagement.Backend.Models;
using Slugify;
using SchoolManagement.Backend.Contexts ;

namespace SchoolManagement.Backend.Database.Factories;

public class AdFactory : Factory<Ad>
{
    public AdFactory(AppDbContext context) : base(context)
    {
    }

    protected override Ad Make()
    {
        var name = faker.Commerce.ProductName();
        var platforms = Context.Platforms.Select(p => p.Id).ToList();
        var branches = Context.Branches.Select(b => b.Id).ToList();
        return new Ad
        {
            Name = name,
            BranchId = faker.PickRandom(branches) ,
            Slug = new SlugHelper().GenerateSlug(name),
            PlatformId = faker.PickRandom(platforms) 
        };
    }
}