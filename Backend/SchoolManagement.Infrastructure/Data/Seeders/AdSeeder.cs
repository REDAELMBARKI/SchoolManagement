using System;
using SchoolManagement.Infrastructure.Data.Factories;
using SchoolManagement.Infrastructure.Data.Seeders;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data ;
namespace SchoolManagement.Infrastructure.Data.Seeders;
public class AdSeeder : Seeder
{

    private readonly AdFactory _adFactory ;
    public AdSeeder(AppDbContext context) : base(context)
    {
        _adFactory = new AdFactory(context);
        
    }

    public override async Task RunAsync()
    {
        var items = await _adFactory.MakeMany(3);
        await Context.Ads.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}
