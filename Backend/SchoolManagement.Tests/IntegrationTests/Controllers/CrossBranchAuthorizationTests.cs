using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Tests.IntegrationTests;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Xunit.Abstractions;

namespace SchoolManagement.Tests.IntegrationTests.Controllers;

public class CrossBranchAuthorizationTests : IntegrationTestBase
{
    private readonly ITestOutputHelper _output;

    public CrossBranchAuthorizationTests(WebApplicationFactoryBase<Program> factory, ITestOutputHelper output) : base(factory, output)
    {
        _output = output;
        SeedRolesAsync().GetAwaiter().GetResult();
    }

    #region Cross-Branch Access Tests

    [Fact]
    public async Task GetStudent_FromDifferentBranch_ReturnsNotFound()
    {
        // Arrange - Create two branches and two students
        var branch1 = await DbContext.Branches.FirstAsync(b => b.Name == "Main Branch");
        var branch2 = await DbContext.Branches.FirstAsync(b => b.Name == "Secondary Branch");
        var genderId = await GetGenderAsync("Male");

        // Create student in branch1
        var student = new Student
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(2000, 1, 1)),
            BranchId = branch1.Id,
            GenderId = genderId
        };
        DbContext.Students.Add(student);
        await DbContext.SaveChangesAsync();

        // User from branch2 trying to access student from branch1
        var userId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(userId, branch2.Id.ToString(), "Supervisor", "user@test.com");

        // Act
        var response = await authenticatedClient.GetAsync($"/api/students/{student.Id}");

        // Assert - Should not find student from different branch
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateStudent_InDifferentBranch_ReturnsForbidden()
    {
        // Arrange
        var branch1 = await DbContext.Branches.FirstAsync(b => b.Name == "Main Branch");
        var branch2 = await DbContext.Branches.FirstAsync(b => b.Name == "Secondary Branch");
        var genderId = await GetGenderAsync("Male");

        // User from branch1 trying to create student in branch2
        var userId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(userId, branch1.Id.ToString(), "Supervisor", "user@test.com");

        var createRequest = new
        {
            firstName = "Jane",
            lastName = "Doe",
            dateOfBirth = "2000-01-01",
            branchId = branch2.Id, // Different branch!
            genderId = genderId
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/students", createRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SuperAdmin_CanAccessAllBranches()
    {
        // Arrange
        var branch1 = await DbContext.Branches.FirstAsync(b => b.Name == "Main Branch");
        var genderId = await GetGenderAsync("Male");

        var student = new Student
        {
            FirstName = "John",
            LastName = "SuperTest",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(2000, 1, 1)),
            BranchId = branch1.Id,
            GenderId = genderId
        };
        DbContext.Students.Add(student);
        await DbContext.SaveChangesAsync();

        // SuperAdmin with null branchId can access any branch
        var userId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(userId, null, "SuperAdmin", "superadmin@test.com");

        // Act
        var response = await authenticatedClient.GetAsync($"/api/students/{student.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Role Escalation Tests

    [Fact]
    public async Task User_CannotAccessAdminEndpoint()
    {
        // Arrange
        var branchId = await GetFirstBranchAsync();
        var genderId = await GetGenderAsync("Male");
        
        var userId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(userId, branchId.ToString(), "User", "user@test.com");

        var createStaffRequest = new CreateStaffUserRequestDto
        {
            Email = "newstaff@test.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "Staff",
            Phone = "1234567890",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            GenderId = genderId,
            Role = "Supervisor",
            Salary = 5000,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/create-staff-user", createStaffRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Supervisor_CannotCreateAdmin()
    {
        // Arrange
        var branchId = await GetFirstBranchAsync();
        var genderId = await GetGenderAsync("Male");
        
        var userId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(userId, branchId.ToString(), "Supervisor", "supervisor@test.com");

        var createStaffRequest = new CreateStaffUserRequestDto
        {
            Email = "newadmin@test.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "Admin",
            Phone = "1234567890",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            GenderId = genderId,
            Role = "Admin", // Trying to create higher role
            Salary = 5000,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/create-staff-user", createStaffRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Admin_CannotCreateSuperAdmin()
    {
        // Arrange
        var branchId = await GetFirstBranchAsync();
        var genderId = await GetGenderAsync("Male");
        
        var userId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(userId, branchId.ToString(), "Admin", "admin@test.com");

        var createStaffRequest = new CreateStaffUserRequestDto
        {
            Email = "newsuperadmin@test.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "SuperAdmin",
            Phone = "1234567890",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            GenderId = genderId,
            Role = "SuperAdmin", // Trying to create SuperAdmin
            Salary = 5000,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/create-staff-user", createStaffRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Missing Headers Tests

    [Fact]
    public async Task Request_WithoutAuthorizationHeader_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/students");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Request_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer invalid-token-here");

        // Act
        var response = await client.GetAsync("/api/students");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Request_WithMissingBranchId_ForNonSuperAdmin_ReturnsForbidden()
    {
        // Arrange - User without branchId claim (but not SuperAdmin)
        var userId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(userId, null, "Supervisor", "user@test.com");

        // Act
        var response = await authenticatedClient.GetAsync("/api/students");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.BadRequest);
    }

    #endregion
}
