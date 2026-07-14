using Bogus;
using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Infrastructure.Data.Factories; using SchoolManagement.Domain.Entities; using SchoolManagement.Infrastructure.Data ; namespace SchoolManagement.Infrastructure.Data.Seeders;

public class GenderSeeder : Seeder
{
    public GenderSeeder( AppDbContext context) :base(context)
    {
    }

    public override async Task RunAsync()
    {
        if (await Context.Genders.AnyAsync()) return;
        List<Gender> items = new List<Gender>
        {
            new Gender
            {
                Name  = "Male",
                Slug = "male"
            },
            new Gender
            {
                Name  = "Female",
                Slug = "female"
            }
        };
        await Context.Genders.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}

