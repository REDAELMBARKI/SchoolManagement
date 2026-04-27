using System;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Database.Seeders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Seeders;
public class AdSeeder : Seeder
{

    private readonly AdFactory _adFactory ;
    public AdSeeder(AppDbContext context) : base(context)
    {
        _adFactory = new AdFactory(context);
        
    }

    public override async Task RunAsync()
    {
        var items =  _adFactory.MakeMany(3);
        await Context.Ads.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}
