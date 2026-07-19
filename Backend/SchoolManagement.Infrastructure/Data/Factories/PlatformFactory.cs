using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class PlatformFactory : Factory<Platform>
{
    public PlatformFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Platform> Make()
    {
        var platforms = new[] { "Facebook", "Google Ads", "TikTok", "Instagram", "YouTube" };
        var name = faker.PickRandom(platforms);
        return Task.FromResult(Platform.Create(
            name: name,
            slug: this.GenerateSlug(name)
        ));
    }
}