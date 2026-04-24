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
        IEnumerable<LeadSource> items =  new LeadSource[]
        {
             new LeadSource
             {
                 Name  = "Ad",
                 AdId  = _context.Ads.OrderBy(a => Guid.NewGuid()).Select(a => a.Id).First()
             } ,
             new LeadSource
             {
                 Name  = "opc",
                 OpcId  = _context.Users.OfType<Opc>().OrderBy(a => Guid.NewGuid()).Select(a => a.Id).First()
             } ,
       
          

        };
        await _context.LeadSources.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }
}