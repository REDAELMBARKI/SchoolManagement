using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Infrastructure.Data;
using System.Net.Http.Headers;
using Xunit;
using Xunit.Abstractions;

namespace SchoolManagement.Tests.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactoryBase<Program>>, IDisposable
{
    protected readonly WebApplicationFactoryBase<Program> Factory;
    protected HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly AppDbContext DbContext;

    protected IntegrationTestBase(WebApplicationFactoryBase<Program> factory, ITestOutputHelper _output)
    {
        Factory = factory;
        try
        {
            Client = factory.CreateClient();
        }
        catch (Exception ex)
        {
            _output.WriteLine(ex.ToString()); // full stack trace, not just ex.Message
            throw;
        }

        // Create a scope to get the DbContext
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // DO NOT set default authentication - let tests control this explicitly
        // Tests that need authentication should call CreateAuthenticatedClient()

    }

    /// <summary>
    /// Creates an authenticated HTTP client with custom claims
    /// </summary>
    protected HttpClient CreateAuthenticatedClient(
        string? userId = null,
        string? branchId = null,
        string? role = null,
        string? userName = null)
    {
        var client = Factory.CreateClient();

        if (userId != null)
            client.DefaultRequestHeaders.Add("X-Test-UserId", userId);

        if (branchId != null)
            client.DefaultRequestHeaders.Add("X-Test-BranchId", branchId);

        if (role != null)
            client.DefaultRequestHeaders.Add("X-Test-Role", role);

        if (userName != null)
            client.DefaultRequestHeaders.Add("X-Test-UserName", userName);

        return client;
    }

    /// <summary>
    /// Creates an unauthenticated HTTP client for testing unauthorized access
    /// </summary>
    protected HttpClient CreateUnauthenticatedClient()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Unauthorized", "true");
        return client;
    }

    /// <summary>
    /// Sets authentication headers on the default client
    /// </summary>
    protected void SetAuthenticationHeaders(
        string userId = "test-user-id",
        string branchId = "test-branch-id",
        string role = "Administrator",
        string userName = "TestUser")
    {
        Client.DefaultRequestHeaders.Remove("X-Test-UserId");
        Client.DefaultRequestHeaders.Remove("X-Test-BranchId");
        Client.DefaultRequestHeaders.Remove("X-Test-Role");
        Client.DefaultRequestHeaders.Remove("X-Test-UserName");

        Client.DefaultRequestHeaders.Add("X-Test-UserId", userId);
        Client.DefaultRequestHeaders.Add("X-Test-BranchId", branchId);
        Client.DefaultRequestHeaders.Add("X-Test-Role", role);
        Client.DefaultRequestHeaders.Add("X-Test-UserName", userName);
    }


    /// <summary>
    /// Clears specific tables from the test database
    /// </summary>
    protected async Task ClearTablesAsync(params string[] tableNames)
    {
        foreach (var tableName in tableNames)
        {
            await DbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [{tableName}]");
        }
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Clears Identity/Auth related tables
    /// </summary>
    protected async Task ClearAuthTablesAsync()
    {
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserTokens]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserRoles]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserLogins]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserClaims]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetRoleClaims]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [RefreshTokens]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUsers]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetRoles]");
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Clears ONLY registered users (keeps roles intact)
    /// Use this to avoid duplicate email errors when testing registration multiple times
    /// </summary>
    protected async Task ClearUsersOnlyAsync()
    {
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserTokens]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserRoles]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserLogins]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserClaims]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [RefreshTokens]");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUsers]");
        // NOTE: AspNetRoles NOT deleted - roles stay for subsequent tests
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds Identity Roles required for user registration
    /// </summary>
    protected async Task SeedRolesAsync()
    {
        // Check if roles already exist
        if (await DbContext.Roles.AnyAsync())
        {
            return; // Already seeded
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
            var role = new Microsoft.AspNetCore.Identity.IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            DbContext.Roles.Add(role);
        }

        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Ensures test database exists and runs migrations
    /// </summary>
    protected async Task EnsureDatabaseAsync()
    {
        await DbContext.Database.MigrateAsync();
    }



    /// <summary>
    /// Seeds basic test data that most tests need
    /// </summary>
    protected async Task SeedBasicTestDataAsync()
    {
        // Override in derived classes to add test-specific seed data
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        Scope?.Dispose();
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets a Gender from the database (must be seeded by DatabaseSeeder)
    /// </summary>
    protected async Task<Guid> GetGenderAsync(string name = "Male")
    {
        var gender = await DbContext.Genders
            .Where(g => g.Name == name)
            .Select(g => g.Id)
            .FirstOrDefaultAsync();
        
        if (gender == Guid.Empty)
        {
            throw new InvalidOperationException($"Gender '{name}' not found. Ensure DatabaseSeeder has run.");
        }
        
        return gender;
    }

    /// <summary>
    /// Gets a Branch from the database (must be seeded by DatabaseSeeder)
    /// </summary>
    protected async Task<Guid> GetBranchAsync(string name = "Test Branch")
    {
        var branch = await DbContext.Branches
            .Where(b => b.Name == name)
            .Select(b => b.Id)
            .FirstOrDefaultAsync();
        
        if (branch == Guid.Empty)
        {
            throw new InvalidOperationException($"Branch '{name}' not found. Ensure DatabaseSeeder has run.");
        }
        
        return branch;
    }

    /// <summary>
    /// Gets the first non-system branch from the database (must be seeded by DatabaseSeeder)
    /// </summary>
    protected async Task<Guid> GetFirstBranchAsync()
    {
        var branch = await DbContext.Branches
            .Where(b => b.Id != SchoolManagement.Domain.Common.Entities.Branch.SYSTEM_BRANCH_ID 
                     && b.Id != SchoolManagement.Domain.Common.Entities.Branch.GLOBAL_USER_BRANCH_ID)
            .Select(b => b.Id)
            .FirstOrDefaultAsync();
        
        if (branch == Guid.Empty)
        {
            throw new InvalidOperationException("No regular branches found. Ensure DatabaseSeeder has run.");
        }
        
        return branch;
    }

    /// <summary>
    /// Seeds Genders if they don't exist
    /// </summary>
    protected async Task SeedGendersAsync()
    {
        if (await DbContext.Genders.AnyAsync())
        {
            return; // Already seeded
        }

        var genders = new[]
        {
            SchoolManagement.Domain.Common.Entities.Gender.Create("Male", "male"),
            SchoolManagement.Domain.Common.Entities.Gender.Create("Female", "female")
        };

        await DbContext.Genders.AddRangeAsync(genders);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds a test branch if it doesn't exist
    /// </summary>
    protected async Task SeedTestBranchAsync()
    {
        var exists = await DbContext.Branches.AnyAsync(b => b.Name == "Test Branch");
        if (!exists)
        {
            var branch = SchoolManagement.Domain.Common.Entities.Branch.Create(
                name: "Test Branch",
                slug: "test-branch",
                city: "Test City",
                address: "123 Test St",
                phone: "1234567890"
            );
            await DbContext.Branches.AddAsync(branch);
            await DbContext.SaveChangesAsync();
        }
    }



}
