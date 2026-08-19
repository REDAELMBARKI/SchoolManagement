using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Infrastructure.Data;
using System.Net.Http.Headers;

namespace SchoolManagement.Tests.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactoryBase<Program>>, IDisposable
{
    protected readonly WebApplicationFactoryBase<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly AppDbContext DbContext;

    protected IntegrationTestBase(WebApplicationFactoryBase<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        
        // Create a scope to get the DbContext
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Set default test authentication
        SetAuthenticationHeaders();
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
}
