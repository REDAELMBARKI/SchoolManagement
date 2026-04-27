using SchoolManagement.Backend.Models;
using Slugify;
namespace SchoolManagement.Backend.Database.Factories;

public class AdFactory : Factory<Ad>
{
    public AdFactory(AppDbContext context) : base(context)
    {
    }

    protected override Ad Make()
    {
        var name = faker.Commerce.ProductName();
        return new Ad
        {
            Name = name,
            Slug = new SlugHelper().GenerateSlug(name),
            PlatformId = Context.Platforms.OrderBy(p => Guid.NewGuid()).Select(p => p.Id).First()
        };
    }
}