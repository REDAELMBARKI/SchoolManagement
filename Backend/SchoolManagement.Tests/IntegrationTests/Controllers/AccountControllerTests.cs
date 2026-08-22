using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.CrossCutting.Identity.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Tests.IntegrationTests;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace SchoolManagement.Tests.IntegrationTests.Controllers;

public class AccountControllerTests : IntegrationTestBase
{
    private readonly ITestOutputHelper _output;

    public AccountControllerTests(WebApplicationFactoryBase<Program> factory, ITestOutputHelper output) : base(factory ,output)
    {
        _output = output;
        
        // Seed roles once in constructor
        _output.WriteLine("Seeding Identity roles...");
        SeedRolesAsync().GetAwaiter().GetResult();
        _output.WriteLine("Roles seeded successfully");
    }

    #region Register Tests

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var request = new RegisterRequestDto
        {
            Email = "newuser@test.com",
            Password = "ValidPassword123!",
            ConfirmPassword = "ValidPassword123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/register", request);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response: {content}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("message").GetString().Should().Contain("Registration successful");
        body.GetProperty("applicationUserId").GetString().Should().NotBeNullOrEmpty();

        // Verify in database
        var userId = body.GetProperty("applicationUserId").GetString();
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        user.Should().NotBeNull();
        user!.Email.Should().Be(request.Email);
        user.EmailConfirmed.Should().BeFalse();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {

        try { 

            _output.WriteLine("=== START TEST ===");
            // Arrange
            await ClearUsersOnlyAsync();

            _output.WriteLine($"prepare request ");
         var request = new RegisterRequestDto
            {
                Email = "duplicate@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _output.WriteLine($"register first time");

            await Client.PostAsJsonAsync("/api/account/register", request);


            _output.WriteLine($"register second time with same email");
            // Act - Try to register again with same email
            var response = await Client.PostAsJsonAsync("/api/account/register", request);
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Response Status: {response.StatusCode}");
            _output.WriteLine($"Response Content: {content}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain("already");

        _output.WriteLine("=== TEST PASSED ===");
        }
        catch (Exception ex) { 
        
           _output.WriteLine($"ex type : {ex.GetType().Name}");
           _output.WriteLine($"ex message : {ex.Message}");
           _output.WriteLine($"ex stacktrace : {ex.StackTrace}");

        }
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var request = new RegisterRequestDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {


        try
        {
            _output.WriteLine("=== START TEST ===");
            _output.WriteLine("Step 1: Clear users");

            // Arrange
            await ClearUsersOnlyAsync();

            _output.WriteLine("Step 2: Prepare weak password request");
            var request = new RegisterRequestDto
            {
                Email = "test@test.com",
                Password = "weak",
                ConfirmPassword = "weak"
            };

            // Act
            _output.WriteLine("Step 3: Send registration request with weak password");
            var response = await Client.PostAsJsonAsync("/api/account/register", request);
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Response Status: {response.StatusCode}");
            _output.WriteLine($"Response Content: {content}");


            _output.WriteLine("Step 4: Assert BadRequest for weak password");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain("password");

            _output.WriteLine("=== TEST PASSED ===");
        }
        catch (Exception ex) { 
        
           _output.WriteLine($"ex type : {ex.GetType().Name}");
           _output.WriteLine($"ex message : {ex.Message}");
           _output.WriteLine($"ex stacktrace : {ex.StackTrace}");

        }
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var request = new RegisterRequestDto
        {
            Email = "invalid-email",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithEmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var request = new RegisterRequestDto
        {
            Email = "",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "logintest@test.com",
            Password = "LoginPassword123!",
            ConfirmPassword = "LoginPassword123!"
        };
        var registerResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var body0 = await registerResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = body0.GetProperty("applicationUserId").GetString();
        _output.WriteLine($"registered user : {userId}");

        // Confirm email
        var user = await DbContext.Users.FirstAsync(u => u.Id == userId);
        user.EmailConfirmed = true;
        await DbContext.SaveChangesAsync();
        _output.WriteLine("Email confirmed");
 
        var loginRequest = new LoginRequestDto
        {
            Email = "logintest@test.com",
            Password = "LoginPassword123!",
            RememberMe = false
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Login Response: {content}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("accessToken").GetString().Should().NotBeNullOrEmpty();
        body.GetProperty("refreshToken").GetString().Should().NotBeNullOrEmpty();
        body.GetProperty("accessTokenExpiresAt").GetDateTime().Should().BeAfter(DateTime.UtcNow);
        body.GetProperty("refreshTokenExpiresAt").GetDateTime().Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var loginRequest = new LoginRequestDto
        {
            Email = "nonexistent@test.com",
            Password = "Password123!",
            RememberMe = false
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "wrongpass@test.com",
            Password = "CorrectPassword123!",
            ConfirmPassword = "CorrectPassword123!"
        };
        await Client.PostAsJsonAsync("/api/account/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            Email = "wrongpass@test.com",
            Password = "WrongPassword123!",
            RememberMe = false
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithRememberMe_ReturnsLongerRefreshTokenExpiry()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "rememberme@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var registerResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await registerResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        // Confirm email
        var user = await DbContext.Users.FirstAsync(u => u.Id == userId);
        user.EmailConfirmed = true;
        await DbContext.SaveChangesAsync();

        var loginRequest = new LoginRequestDto
        {
            Email = "rememberme@test.com",
            Password = "Password123!",
            RememberMe = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/login", loginRequest);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshExpiry = body.GetProperty("refreshTokenExpiresAt").GetDateTime();
        refreshExpiry.Should().BeAfter(DateTime.UtcNow.AddDays(29), 
            "RememberMe should extend refresh token to ~30 days");
    }

    [Fact]
    public async Task Login_WithEmptyCredentials_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "",
            Password = "",
            RememberMe = false
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Change Password Tests

    [Fact]
    public async Task ChangePassword_WithValidCurrentPassword_ReturnsOk()
    {
        try
        {
            _output.WriteLine("=== START TEST ===");
            _output.WriteLine("Step 1: Clear users");
            await ClearUsersOnlyAsync();

            _output.WriteLine("Step 2: Register user");
            var registerRequest = new RegisterRequestDto
            {
                Email = "changepass@test.com",
                Password = "OldPassword123!",
                ConfirmPassword = "OldPassword123!"
            };
            var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
            var regContent = await regResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Register Response Status: {regResponse.StatusCode}");
            _output.WriteLine($"Register Response Body: {regContent}");
            
            var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
            var userId = regBody.GetProperty("applicationUserId").GetString();
            _output.WriteLine($"UserId: {userId}");

            _output.WriteLine("Step 3: Create authenticated client");
            var authenticatedClient = CreateAuthenticatedClient(userId!, null, "User", "changepass@test.com");

            _output.WriteLine("Step 4: Change password");
            var changePasswordRequest = new ChangePasswordRequestDto
            {
                ApplicationUserId = userId!,
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };
            var response = await authenticatedClient.PostAsJsonAsync("/api/account/change-password", changePasswordRequest);
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"ChangePassword Response Status: {response.StatusCode}");
            _output.WriteLine($"ChangePassword Response Body: {content}");

            _output.WriteLine("Step 5: Assert status code");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            body.GetProperty("message").GetString().Should().Contain("changed successfully");

            _output.WriteLine("Step 6: Verify password changed using UserManager");
            using var scope = Factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbUser = await userManager.FindByIdAsync(userId!);
            
            _output.WriteLine("Step 7: Verify old password no longer works");
            var oldPasswordWorks = await userManager.CheckPasswordAsync(dbUser!, "OldPassword123!");
            oldPasswordWorks.Should().BeFalse();
            
            _output.WriteLine("Step 8: Verify new password works");
            var newPasswordWorks = await userManager.CheckPasswordAsync(dbUser!, "NewPassword123!");
            newPasswordWorks.Should().BeTrue();
            
            _output.WriteLine("=== TEST PASSED ===");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"=== EXCEPTION ===");
            _output.WriteLine($"Type: {ex.GetType().Name}");
            _output.WriteLine($"Message: {ex.Message}");
            _output.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                _output.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_ReturnsBadRequest()
    {
        try
        {
            // Arrange
            await ClearUsersOnlyAsync();
            
            var registerRequest = new RegisterRequestDto
            {
                Email = "wrongcurrent@test.com",
                Password = "CorrectPassword123!",
                ConfirmPassword = "CorrectPassword123!"
            };
            var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
            var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
            var userId = regBody.GetProperty("applicationUserId").GetString();

            var authenticatedClient = CreateAuthenticatedClient(userId!, null, "User", "wrongcurrent@test.com");

            var changePasswordRequest = new ChangePasswordRequestDto
            {
                ApplicationUserId = userId!,
                CurrentPassword = "WrongPassword123!",
                NewPassword = "NewPassword123!"
            };

            // Act
            var response = await authenticatedClient.PostAsJsonAsync("/api/account/change-password", changePasswordRequest);
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response: {content}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _output.WriteLine($"=== EXCEPTION IN ChangePassword_WithWrongCurrentPassword_ReturnsBadRequest ===");
            _output.WriteLine($"Exception Type: {ex.GetType().Name}");
            _output.WriteLine($"Message: {ex.Message}");
            _output.WriteLine($"Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                _output.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                _output.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
            }
            throw;
        }
    }

    [Fact]
    public async Task ChangePassword_WithWeakNewPassword_ReturnsBadRequest()
    {
        try
        {
            _output.WriteLine("=== START TEST ===");
            _output.WriteLine("Step 1: Clear users");
            await ClearUsersOnlyAsync();
            
            _output.WriteLine("Step 2: Register user");
            var registerRequest = new RegisterRequestDto
            {
                Email = "weaknew@test.com",
                Password = "OldPassword123!",
                ConfirmPassword = "OldPassword123!"
            };
            var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
            var regContent = await regResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Register Response Status: {regResponse.StatusCode}");
            _output.WriteLine($"Register Response Body: {regContent}");

            var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
            var userId = regBody.GetProperty("applicationUserId").GetString();
            _output.WriteLine($"UserId: {userId}");

            _output.WriteLine("Step 3: Create authenticated client");
            var authenticatedClient = CreateAuthenticatedClient(userId!, null, "User", "weaknew@test.com");

            _output.WriteLine("Step 4: Change password with weak password");
            var changePasswordRequest = new ChangePasswordRequestDto
            {
                ApplicationUserId = userId!,
                CurrentPassword = "OldPassword123!",
                NewPassword = "weak"
            };

            var response = await authenticatedClient.PostAsJsonAsync("/api/account/change-password", changePasswordRequest);
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response Status: {response.StatusCode}");
            _output.WriteLine($"Response Body: {content}");

            _output.WriteLine("Step 5: Assert BadRequest");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            _output.WriteLine("=== TEST PASSED ===");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"=== EXCEPTION ===");
            _output.WriteLine($"Type: {ex.GetType().Name}");
            _output.WriteLine($"Message: {ex.Message}");
            _output.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                _output.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    [Fact]
    public async Task ChangePassword_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var changePasswordRequest = new ChangePasswordRequestDto
        {
            ApplicationUserId = Guid.NewGuid().ToString(),
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Forgot Password Tests

    [Fact]
    public async Task ForgotPassword_WithExistingEmail_ReturnsOk()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "forgot@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        await Client.PostAsJsonAsync("/api/account/register", registerRequest);

        var forgotPasswordRequest = new ForgotPasswordRequestDto
        {
            Email = "forgot@test.com"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/forgot-password", forgotPasswordRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("message").GetString().Should().Contain("password reset");
    }

    [Fact]
    public async Task ForgotPassword_WithNonExistentEmail_ReturnsOkForSecurity()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var forgotPasswordRequest = new ForgotPasswordRequestDto
        {
            Email = "nonexistent@test.com"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/forgot-password", forgotPasswordRequest);

        // Assert
        // Should return OK to prevent email enumeration attacks
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPassword_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var forgotPasswordRequest = new ForgotPasswordRequestDto
        {
            Email = "invalid-email"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/forgot-password", forgotPasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Reset Password Tests

    [Fact]
    public async Task ResetPassword_WithValidToken_ReturnsOk()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        // Register user
        var registerRequest = new RegisterRequestDto
        {
            Email = "reset@test.com",
            Password = "OldPassword123!",
            ConfirmPassword = "OldPassword123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        // Confirm email first
        var user1 = await DbContext.Users.FirstAsync(u => u.Id == userId);
        user1.EmailConfirmed = true;
        await DbContext.SaveChangesAsync();

        // Request password reset
        await Client.PostAsJsonAsync("/api/account/forgot-password", new ForgotPasswordRequestDto
        {
            Email = "reset@test.com"
        });

        // Get token from database (in real scenario, it would come from email)
        var user = await DbContext.Users.FirstAsync(u => u.Id == userId);
        var userManager = Factory.Services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var resetPasswordRequest = new ResetPasswordWithTokenRequestDto
        {
            ApplicationUserId = userId!,
            Token = token,
            NewPassword = "NewPassword123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/reset-password", resetPasswordRequest);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"ResetPassword Response: {content}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify can login with new password
        var loginResponse = await Client.PostAsJsonAsync("/api/account/login", new LoginRequestDto
        {
            Email = "reset@test.com",
            Password = "NewPassword123!",
            RememberMe = false
        });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPassword_WithInvalidToken_ReturnsBadRequest()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "invalidtoken@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        var resetPasswordRequest = new ResetPasswordWithTokenRequestDto
        {
            ApplicationUserId = userId!,
            Token = "invalid-token",
            NewPassword = "NewPassword123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/reset-password", resetPasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_WithWeakNewPassword_ReturnsBadRequest()
    {
        // Arrange
        var resetPasswordRequest = new ResetPasswordWithTokenRequestDto
        {
            ApplicationUserId = Guid.NewGuid().ToString(),
            Token = "some-token",
            NewPassword = "weak"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/reset-password", resetPasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Email Confirmation Tests

    [Fact]
    public async Task ConfirmEmail_WithValidToken_RedirectsWithSuccess()
    {
        try
        {
            _output.WriteLine("=== START TEST ===");
            _output.WriteLine("Step 1: Clear users");
            await ClearUsersOnlyAsync();
            
            _output.WriteLine("Step 2: Register user");
            var registerRequest = new RegisterRequestDto
            {
                Email = "confirm@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
            var regContent = await regResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Register Response Status: {regResponse.StatusCode}");
            _output.WriteLine($"Register Response Body: {regContent}");
            
            var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
            var userId = regBody.GetProperty("applicationUserId").GetString();
            _output.WriteLine($"UserId: {userId}");

            _output.WriteLine("Step 3: Generate email confirmation token");
            var user = await DbContext.Users.FirstAsync(u => u.Id == userId);
            var userManager = Factory.Services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            _output.WriteLine($"Token generated (length: {token.Length})");

            _output.WriteLine("Step 4: Create client that doesn't follow redirects");
            var noRedirectClient = Factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _output.WriteLine("Step 5: Confirm email with valid token");
            var response = await noRedirectClient.GetAsync($"/api/account/confirm-email?userId={userId}&token={Uri.EscapeDataString(token)}");
            _output.WriteLine($"Response Status: {response.StatusCode}");
            _output.WriteLine($"Response Location: {response.Headers.Location}");

            _output.WriteLine("Step 6: Assert redirect");
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.ToString().Should().Contain("emailConfirmed=true");

            _output.WriteLine("Step 7: Verify in database");
            // Detach any tracked entities and reload fresh from database
            DbContext.ChangeTracker.Clear();
            var confirmedUser = await DbContext.AspNetUsers.AsNoTracking().FirstAsync(u => u.Id == userId);
            _output.WriteLine($"EmailConfirmed value: {confirmedUser.EmailConfirmed}");
            confirmedUser.EmailConfirmed.Should().BeTrue();
            
            _output.WriteLine("=== TEST PASSED ===");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"=== EXCEPTION ===");
            _output.WriteLine($"Type: {ex.GetType().Name}");
            _output.WriteLine($"Message: {ex.Message}");
            _output.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                _output.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    [Fact]
    public async Task ConfirmEmail_WithInvalidToken_RedirectsWithError()
    {
        try
        {
            _output.WriteLine("=== START TEST ===");
            _output.WriteLine("Step 1: Clear users");
            await ClearUsersOnlyAsync();
            
            _output.WriteLine("Step 2: Register user");
            var registerRequest = new RegisterRequestDto
            {
                Email = "invalidconfirm@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
            var regContent = await regResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Register Response Status: {regResponse.StatusCode}");
            _output.WriteLine($"Register Response Body: {regContent}");
            
            var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
            var userId = regBody.GetProperty("applicationUserId").GetString();
            _output.WriteLine($"UserId: {userId}");

            _output.WriteLine("Step 3: Create client that doesn't follow redirects");
            var noRedirectClient = Factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _output.WriteLine("Step 4: Confirm email with invalid token");
            var response = await noRedirectClient.GetAsync($"/api/account/confirm-email?userId={userId}&token=invalid-token");
            _output.WriteLine($"Response Status: {response.StatusCode}");
            _output.WriteLine($"Response Location: {response.Headers.Location}");

            _output.WriteLine("Step 5: Assert redirect");
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location!.ToString().Should().Contain("emailConfirmed=false");

            _output.WriteLine("=== TEST PASSED ===");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"=== EXCEPTION ===");
            _output.WriteLine($"Type: {ex.GetType().Name}");
            _output.WriteLine($"Message: {ex.Message}");
            _output.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                _output.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    [Fact]
    public async Task ConfirmEmail_WithMissingUserId_ReturnsBadRequest()
    {
        // Act
        var response = await Client.GetAsync("/api/account/confirm-email?token=some-token");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResendConfirmationEmail_WithValidEmail_ReturnsOk()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "resend@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        await Client.PostAsJsonAsync("/api/account/register", registerRequest);

        var resendRequest = new ResendConfirmationEmailRequestDto
        {
            Email = "resend@test.com"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/resend-confirmation-email", resendRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("message").GetString().Should().Contain("confirmation");
    }

    [Fact]
    public async Task ResendConfirmationEmail_ForAlreadyConfirmedEmail_ReturnsBadRequest()
    {

        _output.WriteLine("clear users ");
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "alreadyconfirmed@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        _output.WriteLine("register user the first time  ");

        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        _output.WriteLine("confimring  his mail adress fist time   ");

        // Confirm email directly in database to avoid tracking conflicts
        await DbContext.Database.ExecuteSqlRawAsync($"UPDATE AspNetUsers SET EmailConfirmed = 1 WHERE Id = '{userId}'");


        var resendRequest = new ResendConfirmationEmailRequestDto
        {
            Email = "alreadyconfirmed@test.com"
        };

        _output.WriteLine("send confirmation email token  request ");
        // Act
        var response = await Client.PostAsJsonAsync("/api/account/resend-confirmation-email", resendRequest);

        var body = response.Content.ReadAsStringAsync();
        _output.WriteLine($"status code : {response.StatusCode}");
        _output.WriteLine($"res content  : {body}");

        _output.WriteLine("begin assertion   ");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("error").GetString().Should().Contain("already confirmed");
    }

    [Fact]
    public async Task ResendConfirmationEmail_WithNonExistentEmail_ReturnsOkForSecurity()
    {
        // Arrange
        var resendRequest = new ResendConfirmationEmailRequestDto
        {
            Email = "nonexistent@test.com"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/resend-confirmation-email", resendRequest);

        // Assert
        // Should return OK to prevent email enumeration attacks (don't reveal if email exists)
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Refresh Token Tests

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        // Register and login
        var registerRequest = new RegisterRequestDto
        {
            Email = "refreshtoken@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        // Confirm email
        var user = await DbContext.Users.FirstAsync(u => u.Id == userId);
        user.EmailConfirmed = true;
        await DbContext.SaveChangesAsync();

        var loginResponse = await Client.PostAsJsonAsync("/api/account/login", new LoginRequestDto
        {
            Email = "refreshtoken@test.com",
            Password = "Password123!",
            RememberMe = false
        });
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var refreshToken = loginBody.GetProperty("refreshToken").GetString();

        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = refreshToken!
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/refresh-token", refreshRequest);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"RefreshToken Response: {content}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("accessToken").GetString().Should().NotBeNullOrEmpty();
        body.GetProperty("refreshToken").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/refresh-token", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RevokeToken_WithValidToken_ReturnsOk()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        // Register and login
        var registerRequest = new RegisterRequestDto
        {
            Email = "revoketoken@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        // Confirm email
        var user = await DbContext.Users.FirstAsync(u => u.Id == userId);
        user.EmailConfirmed = true;
        await DbContext.SaveChangesAsync();

        var loginResponse = await Client.PostAsJsonAsync("/api/account/login", new LoginRequestDto
        {
            Email = "revoketoken@test.com",
            Password = "Password123!",
            RememberMe = false
        });
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var refreshToken = loginBody.GetProperty("refreshToken").GetString();

        // Authenticate with proper parameters (userId, branchId can be null for public users)
        var authenticatedClient = CreateAuthenticatedClient(userId!, null, "User", "revoketoken@test.com");

        var revokeRequest = new RefreshTokenRequestDto
        {
            RefreshToken = refreshToken!
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/revoke-token", revokeRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify token cannot be used anymore
        var refreshRequest = new RefreshTokenRequestDto { RefreshToken = refreshToken! };
        var refreshResponse = await Client.PostAsJsonAsync("/api/account/refresh-token", refreshRequest);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Create Staff User Tests

    [Fact]
    public async Task CreateStaffUser_AsDirector_ReturnsCreated()
    {
        try
        {
            _output.WriteLine("=== START TEST ===");
            
            _output.WriteLine("Step 1: Clear users");
            await ClearUsersOnlyAsync();
            
            _output.WriteLine("Step 2: Get seeded data (Gender, Branch)");
            var genderId = await DbContext.Genders.Where(g => g.Name == "Male").Select(g => g.Id).FirstAsync();
            var branchId = await DbContext.Branches.Where(b => b.Id != Branch.SYSTEM_BRANCH_ID && b.Id != Branch.GLOBAL_USER_BRANCH_ID).Select(b => b.Id).FirstAsync();
            var directorId = Guid.NewGuid().ToString();
            _output.WriteLine($"GenderId: {genderId}");
            _output.WriteLine($"BranchId: {branchId}");
            _output.WriteLine($"DirectorId: {directorId}");
            _output.WriteLine($"Branch.SYSTEM_BRANCH_ID: {Branch.SYSTEM_BRANCH_ID}");
            _output.WriteLine($"Branch.GLOBAL_USER_BRANCH_ID: {Branch.GLOBAL_USER_BRANCH_ID}");
            
            _output.WriteLine("Step 3: Create authenticated client as Director");
            var authenticatedClient = CreateAuthenticatedClient(directorId, branchId.ToString(), "Director", "director@test.com");

            _output.WriteLine("Step 4: Prepare create staff request");
            var uniqueEmail = $"newstaff{Guid.NewGuid():N}@test.com"; // Dynamic email
            var request = new CreateStaffUserRequestDto
            {
                Email = uniqueEmail,
                Password = "Password123!",
                FirstName = "New",
                LastName = "Staff",
                Phone = "1234567890",
                DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
                GenderId = genderId,
                Role = "Administrator",
                BranchId = branchId
            };

            _output.WriteLine("Step 5: Create staff user");
            var response = await authenticatedClient.PostAsJsonAsync("/api/account/create-staff-user", request);
            var content = await response.Content.ReadAsStringAsync(); 
            _output.WriteLine($"Response Status: {response.StatusCode}");
            _output.WriteLine($"Response Body: {content}");

            _output.WriteLine("Step 6: Assert Created");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            _output.WriteLine("=== TEST PASSED ===");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"=== EXCEPTION ===");
            _output.WriteLine($"Type: {ex.GetType().Name}");
            _output.WriteLine($"Message: {ex.Message}");
            _output.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                _output.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    [Fact]
    public async Task CreateStaffUser_AsUser_ReturnsForbidden()
    {
        // Arrange - get seeded data
        var genderId = await DbContext.Genders.Where(g => g.Name == "Male").Select(g => g.Id).FirstAsync();
        var branchId = await DbContext.Branches.Where(b => b.Id != Branch.SYSTEM_BRANCH_ID && b.Id != Branch.GLOBAL_USER_BRANCH_ID).Select(b => b.Id).FirstAsync();
        
        var userId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(userId, branchId.ToString(), "User", "user@test.com");

        var request = new CreateStaffUserRequestDto
        {
            Email = "stafftest@test.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "Staff",
            Phone = "1234567890",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            GenderId = genderId,
            Role = "Supervisor",
            BranchId = branchId
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/create-staff-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateStaffUser_AsUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange - get seeded data
        var genderId = await DbContext.Genders.Where(g => g.Name == "Male").Select(g => g.Id).FirstAsync();
        var branchId = await DbContext.Branches.Where(b => b.Id != Branch.SYSTEM_BRANCH_ID && b.Id != Branch.GLOBAL_USER_BRANCH_ID).Select(b => b.Id).FirstAsync();
        
        var request = new CreateStaffUserRequestDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User",
            Phone = "1234567890",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            GenderId = genderId,
            Role = "Supervisor",
            BranchId = branchId
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/create-staff-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Convert To Staff Tests

    [Fact]
    public async Task ConvertToStaff_AsAdmin_ReturnsOk()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var branchId = await GetFirstBranchAsync();
        var genderId = await GetGenderAsync("Male");

        // Create regular user
        var registerRequest = new RegisterRequestDto
        {
            Email = "convertuser@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        // Admin client
        var adminId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(adminId, branchId.ToString(), "Admin", "admin@test.com");

        var convertRequest = new SchoolManagement.Application.Common.Dtos.Requests.ConvertToStaffRequestDto
        {
            UserId = userId!,
            FirstName = "John",
            LastName = "Doe",
            Phone = "1234567890",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            GenderId = genderId,
            Role = "Supervisor",
            Salary = 5000,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var response = await authenticatedClient.PutAsJsonAsync($"/api/account/convert-to-staff/{userId}", convertRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ConvertToStaff_AsUser_ReturnsForbidden()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var branchId = await GetFirstBranchAsync();
        var genderId = await GetGenderAsync("Male");

        var registerRequest = new RegisterRequestDto
        {
            Email = "convertuser2@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        // User client (not admin)
        var regularUserId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(regularUserId, branchId.ToString(), "User", "user@test.com");

        var convertRequest = new SchoolManagement.Application.Common.Dtos.Requests.ConvertToStaffRequestDto
        {
            UserId = userId!,
            FirstName = "John",
            LastName = "Doe",
            Phone = "1234567890",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            GenderId = genderId,
            Role = "Supervisor",
            Salary = 5000,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var response = await authenticatedClient.PutAsJsonAsync($"/api/account/convert-to-staff/{userId}", convertRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ConvertToStaff_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var branchId = await GetFirstBranchAsync();
        var genderId = await GetGenderAsync("Male");
        var adminId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(adminId, branchId.ToString(), "Admin", "admin@test.com");

        var convertRequest = new SchoolManagement.Application.Common.Dtos.Requests.ConvertToStaffRequestDto
        {
            UserId = Guid.NewGuid().ToString(),
            FirstName = "John",
            LastName = "Doe",
            Phone = "1234567890",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
            GenderId = genderId,
            Role = "Supervisor",
            Salary = 5000,
            HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var response = await authenticatedClient.PutAsJsonAsync($"/api/account/convert-to-staff/{convertRequest.UserId}", convertRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Get User Tests
    [Fact]
    public async Task GetUserById_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync($"/api/account/user/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}
