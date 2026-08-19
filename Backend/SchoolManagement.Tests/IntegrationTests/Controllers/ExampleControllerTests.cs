using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;

namespace SchoolManagement.Tests.IntegrationTests.Controllers;

public class ExampleControllerTests : IntegrationTestBase
{
    public ExampleControllerTests(WebApplicationFactoryBase<Program> factory) 
        : base(factory)
    {
    }

    // Example: Test with default authentication (Administrator in test-branch-id)
    [Fact]
    public async Task GetEndpoint_WithDefaultAuth_ReturnsSuccess()
    {
        // Arrange - using default test user from base class
        
        // Act
        var response = await Client.GetAsync("/api/branches");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Example: Test with custom user/branch/role
    [Fact]
    public async Task GetEndpoint_WithCustomAuth_ReturnsSuccess()
    {
        // Arrange
        var customClient = CreateAuthenticatedClient(
            userId: "custom-user-123",
            branchId: "custom-branch-456", 
            role: "Teacher",
            userName: "TestTeacher"
        );
        
        // Act
        var response = await customClient.GetAsync("/api/branches");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Example: Test unauthorized access
    [Fact]
    public async Task GetEndpoint_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var unauthClient = CreateUnauthenticatedClient();
        
        // Act
        var response = await unauthClient.GetAsync("/api/branches");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Example: Test with database seeding
    [Fact]
    public async Task GetEndpoint_WithSeededData_ReturnsData()
    {
        // Arrange
        await EnsureDatabaseAsync();
        // TODO: Add seed data to DbContext
        // DbContext.Students.Add(new Student { ... });
        // await DbContext.SaveChangesAsync();
        
        // Act
        var response = await Client.GetAsync("/api/students");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
