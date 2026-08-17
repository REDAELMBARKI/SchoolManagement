using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data.Factories;

namespace SchoolManagement.Infrastructure.Data.Seeders;

public class BranchSeeder : Seeder
{
    private readonly AppDbContext _context;
    private readonly BranchFactory _factory;
    
    public BranchSeeder(AppDbContext context, BranchFactory factory) : base(context)
    {
        _context = context;
        _factory = factory;
    }

    public override async Task RunAsync()
    {
        // First, seed SYSTEM_BRANCH_ID if it doesn't exist
        var systemBranchExists = await _context.Branches
            .AnyAsync(b => b.Id == Branch.SYSTEM_BRANCH_ID);

        if (!systemBranchExists)
        {
            var systemBranch = Branch.Create(
                name: "SYSTEM",
                slug: "system",
                city: "System",
                address: "N/A",
                phone: null
            );
            
            // Set the specific SYSTEM_BRANCH_ID
            typeof(Branch).GetProperty("Id")!.SetValue(systemBranch, Branch.SYSTEM_BRANCH_ID);

            await _context.Branches.AddAsync(systemBranch);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"✓ Seeded SYSTEM_BRANCH_ID: {Branch.SYSTEM_BRANCH_ID}");
        }

        // Then seed regular branches
        var existingCount = await _context.Branches.CountAsync();
        if (existingCount <= 1) // Only SYSTEM branch exists
        {
            var items = await _factory.MakeMany(10);
            await Context.Branches.AddRangeAsync(items);
            await Context.SaveChangesAsync();
            
            Console.WriteLine($"✓ Seeded {items.Count} regular branches");
        }
    }
}