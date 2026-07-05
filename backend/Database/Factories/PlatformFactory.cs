using SchoolManagement.Backend.Models;
using Slugify;
using SchoolManagement.Backend.Contexts ;

namespace SchoolManagement.Backend.Database.Factories;

public class PlatformFactory : Factory<Platform>
{
    public PlatformFactory(AppDbContext context) : base(context)
    {
    }

    protected override  Task<Platform> Make()
    {
       var platforms = new[] { "Facebook", "Google Ads", "TikTok", "Instagram", "YouTube" };
        var name = faker.PickRandom(platforms);
        var platform = new Platform
        {
            Name = name,
            Slug = new SlugHelper().GenerateSlug(name)
        };
        return Task.FromResult(platform);
    }
}