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
        var branchIds = _context.Branches.Select(b => b.Id).ToList() ;
        
        IEnumerable<LeadSource> items =  new LeadSource[]
        {


             new LeadSource
             {
                 AdId  = Faker.PickRandom(adsIds),
                 BranchId =  Faker.PickRandom(branchIds),
             } ,
             new LeadSource
             {
                 OpcId  = Faker.PickRandom(opcIds),
                 BranchId =  Faker.PickRandom(branchIds),

             } ,

       
          

        };
        await _context.LeadSources.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }
}