using SchoolManagement.Backend.Entities;
using Slugify;
using SchoolManagement.Backend.Data ;

namespace SchoolManagement.Backend.Data.Factories;

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
            Slug = this.GenerateSlug(name)
        };
        return Task.FromResult(platform);
    }
}