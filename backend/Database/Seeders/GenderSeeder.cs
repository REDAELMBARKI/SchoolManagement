using Bogus;
using Bogus.DataSets;
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;
namespace SchoolManagement.Backend.Database.Seeders;

public class GenderSeeder : Seeder
{
    public GenderSeeder( AppDbContext context) :base(context)
    {
    }

    public override async Task RunAsync()
    {  
        List<Gender> items = new List<Gender>
        {
            new Gender 
            {
                Name  = "Male"
            },
            new Gender 
            {
                Name  = "Female",
            }
        } ;
        await Context.Genders.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}

