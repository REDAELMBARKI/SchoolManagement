using System;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Data.Factories;
using SchoolManagement.Backend.Data.Seeders;
using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;
namespace SchoolManagement.Backend.Data.Seeders;
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
