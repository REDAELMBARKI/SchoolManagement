using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
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
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        var branchId = branchIds.Any() ? faker.PickRandom(branchIds) : Guid.Empty;
        return Task.FromResult(Platform.Create(
            name: name,
            slug: this.GenerateSlug(name),
            branchId: branchId
        ));
    }
}