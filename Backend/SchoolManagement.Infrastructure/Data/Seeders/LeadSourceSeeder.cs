using Bogus;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Seeders;

public class LeadSourceSeeder : Seeder
{
    public LeadSourceSeeder(AppDbContext context) : base(context)
    {
    }

    public override async Task RunAsync()
    {
        var adsIds = await Context.Ads.Select(a => a.Id).ToListAsync();
        var opcIds = await Context.Opcs.Select(a => a.Id).ToListAsync();
        var branchIds = await Context.Branches.Select(b => b.Id).ToListAsync();

        if (!adsIds.Any() || !opcIds.Any() || !branchIds.Any())
            return;

        IEnumerable<LeadSource> items = new LeadSource[]
        {
            AdLeadSource.Create(
                branchId: Faker.PickRandom(branchIds),
                adId: Faker.PickRandom(adsIds)
            ),
            OpcLeadSource.Create(
                branchId: Faker.PickRandom(branchIds),
                opcId: Faker.PickRandom(opcIds)
            )
        };

        await Context.LeadSources.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}