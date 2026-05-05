using SchoolManagement.Backend.Models;
using Slugify;
namespace SchoolManagement.Backend.Database.Factories;

public class PlatformFactory : Factory<Platform>
{
    public PlatformFactory(AppDbContext context) : base(context)
    {
    }

    protected override Platform Make()
    {
       var platforms = new[] { "Facebook", "Google Ads", "TikTok", "Instagram", "YouTube" };
        var name = faker.PickRandom(platforms);
        return new Platform
        {
            Name = name,
            Slug = new SlugHelper().GenerateSlug(name)
        };
    }
}