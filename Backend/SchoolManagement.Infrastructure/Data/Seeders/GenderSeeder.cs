using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Seeders;

public class GenderSeeder : Seeder
{
    public GenderSeeder(AppDbContext context) : base(context)
    {
    }

    public override async Task RunAsync()
    {
        if (await Context.Genders.AnyAsync()) return;
        List<Gender> items = new List<Gender>
        {
            Gender.Create("Male", "male"),
            Gender.Create("Female", "female")
        };
        await Context.Genders.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}

