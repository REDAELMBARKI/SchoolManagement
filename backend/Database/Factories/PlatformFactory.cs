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
        var branches = Context.Branches.Select(b => b.Id).ToList();

        return new Platform
        {
            Name = name,
            BranchId = faker.PickRandom(branches) ,

            Slug = new SlugHelper().GenerateSlug(name)
        };
    }
}