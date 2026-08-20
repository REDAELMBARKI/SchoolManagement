using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Infrastructure.Data.Seeders;

public class RoleSeeder : Seeder
{
    public RoleSeeder(AppDbContext context) : base(context)
    {
    }

    public override async Task RunAsync()
    {
        // Check if roles already exist
        if (await Context.Roles.AnyAsync())
        {
            Console.WriteLine("✓ Roles already seeded, skipping...");
            return;
        }

        var roles = new[]
        {
            "SuperAdmin",
            "Director",
            "Administrator",
            "Receptionist",
            "Teacher",
            "User" // For students/parents
        };

        foreach (var roleName in roles)
        {
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            await Context.Roles.AddAsync(role);
            Console.WriteLine($"✓ Seeded role: {roleName}");
        }

        await Context.SaveChangesAsync();
        Console.WriteLine("✓ All Identity roles seeded successfully");
    }
}
