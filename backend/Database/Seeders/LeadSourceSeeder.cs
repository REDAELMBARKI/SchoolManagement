using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Seeders;

public class LeadSourceSeeder : Seeder
{
    private readonly AppDbContext _context;

    public LeadSourceSeeder(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task RunAsync()
    {  
        var adsIds = _context.Ads.Select(a => a.Id).ToList() ;
        var opcIds = _context.Opcs.Select(a => a.Id).ToList() ;
        IEnumerable<LeadSource> items =  new LeadSource[]
        {
             new LeadSource
             {
                 Name  = "Ad",
                 AdId  = Faker.PickRandom(adsIds)
             } ,
             new LeadSource
             {
                 Name  = "opc",
                 OpcId  = Faker.PickRandom(opcIds)
             } ,
       
          

        };
        await _context.LeadSources.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }
}