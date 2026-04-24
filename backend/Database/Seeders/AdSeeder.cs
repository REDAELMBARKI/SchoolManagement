using System;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Database.Seeders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Seeders;
public class AdSeeder : Seeder
{
    public AdSeeder(AppDbContext context) : base(context)
    {
    }

    public override async Task RunAsync()
    {
        var items = new List<Ad>
        {
            new Ad { Name = "Google Ads" },
            new Ad { Name = "Facebook Ads" },
            new Ad { Name = "Instagram Ads" },
            new Ad { Name = "LinkedIn Ads" },
            new Ad { Name = "Twitter Ads" },
            new Ad { Name = "YouTube Ads" },
            new Ad { Name = "TikTok Ads" },
            new Ad { Name = "Snapchat Ads" },
            new Ad { Name = "Pinterest Ads" },
            new Ad { Name = "Reddit Ads" }
        };

        await Context.Ads.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}
