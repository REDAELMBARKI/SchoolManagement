using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Tests.IntegrationTests;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace SchoolManagement.Tests.UnitTests.Controllers;

public class AccountControllerTests : IntegrationTestBase
{
    private readonly ITestOutputHelper _output;

    public AccountControllerTests(WebApplicationFactoryBase<Program> factory, ITestOutputHelper output) : base(factory)
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
        // Arrange
        await ClearUsersOnlyAsync();
        
        var request = new RegisterRequestDto
        {
            Email = "duplicate@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        await Client.PostAsJsonAsync("/api/account/register", request);

        // Act - Try to register again with same email
        var response = await Client.PostAsJsonAsync("/api/account/register", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Contain("already");
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
        // Arrange
        await ClearUsersOnlyAsync();
        
        var request = new RegisterRequestDto
        {
            Email = "test@test.com",
            Password = "weak",
            ConfirmPassword = "weak"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/register", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Contain("password");
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
        await Client.PostAsJsonAsync("/api/account/register", registerRequest);

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
        await Client.PostAsJsonAsync("/api/account/register", registerRequest);

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
        refreshExpiry.Should().BeAfter(DateTime.UtcNow.AddDays(20), 
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
        // Arrange
        await ClearUsersOnlyAsync();
        
        // Register and login to get authenticated client
        var registerRequest = new RegisterRequestDto
        {
            Email = "changepass@test.com",
            Password = "OldPassword123!",
            ConfirmPassword = "OldPassword123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        var authenticatedClient = CreateAuthenticatedClient(userId!, Guid.Empty, "User", "changepass@test.com");

        var changePasswordRequest = new ChangePasswordRequestDto
        {
            ApplicationUserId = userId!,
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/change-password", changePasswordRequest);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"ChangePassword Response: {content}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("message").GetString().Should().Contain("changed successfully");

        // Verify can login with new password
        var loginRequest = new LoginRequestDto
        {
            Email = "changepass@test.com",
            Password = "NewPassword123!",
            RememberMe = false
        };
        var loginResponse = await Client.PostAsJsonAsync("/api/account/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_ReturnsBadRequest()
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

        var authenticatedClient = CreateAuthenticatedClient(userId!, Guid.Empty, "User", "wrongcurrent@test.com");

        var changePasswordRequest = new ChangePasswordRequestDto
        {
            ApplicationUserId = userId!,
            CurrentPassword = "WrongPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_WithMismatchedNewPasswords_ReturnsBadRequest()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "mismatch@test.com",
            Password = "OldPassword123!",
            ConfirmPassword = "OldPassword123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        var authenticatedClient = CreateAuthenticatedClient(userId!, Guid.Empty, "User", "mismatch@test.com");

        var changePasswordRequest = new ChangePasswordRequestDto
        {
            ApplicationUserId = userId!,
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var changePasswordRequest = new ChangePasswordRequestDto
        {
            ApplicationUserId = Guid.NewGuid().ToString(),
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
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

        // Request password reset
        await Client.PostAsJsonAsync("/api/account/forgot-password", new ForgotPasswordRequestDto
        {
            Email = "reset@test.com"
        });

        // Get token from database (in real scenario, it would come from email)
        var user = await DbContext.Users.FirstAsync(u => u.Id == userId);
        var token = await Factory.Services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<SchoolManagement.Domain.Common.Entities.ApplicationUser>>()
            .GeneratePasswordResetTokenAsync(user);

        var resetPasswordRequest = new ResetPasswordRequestDto
        {
            Email = "reset@test.com",
            Token = token,
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
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
        await Client.PostAsJsonAsync("/api/account/register", registerRequest);

        var resetPasswordRequest = new ResetPasswordRequestDto
        {
            Email = "invalidtoken@test.com",
            Token = "invalid-token",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/reset-password", resetPasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_WithMismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        var resetPasswordRequest = new ResetPasswordRequestDto
        {
            Email = "test@test.com",
            Token = "some-token",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "DifferentPassword123!"
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
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "confirm@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        // Generate token
        var user = await DbContext.Users.FirstAsync(u => u.Id == userId);
        var userManager = Factory.Services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<SchoolManagement.Domain.Common.Entities.ApplicationUser>>();
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        // Act
        var response = await Client.GetAsync($"/api/account/confirm-email?userId={userId}&token={Uri.EscapeDataString(token)}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("emailConfirmed=true");

        // Verify in database
        var confirmedUser = await DbContext.Users.FirstAsync(u => u.Id == userId);
        confirmedUser.EmailConfirmed.Should().BeTrue();
    }

    [Fact]
    public async Task ConfirmEmail_WithInvalidToken_RedirectsWithError()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "invalidconfirm@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        // Act
        var response = await Client.GetAsync($"/api/account/confirm-email?userId={userId}&token=invalid-token");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("emailConfirmed=false");
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
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "alreadyconfirmed@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        // Confirm email
        var user = await DbContext.Users.FirstAsync(u => u.Id == userId);
        var userManager = Factory.Services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<SchoolManagement.Domain.Common.Entities.ApplicationUser>>();
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await userManager.ConfirmEmailAsync(user, token);

        var resendRequest = new ResendConfirmationEmailRequestDto
        {
            Email = "alreadyconfirmed@test.com"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/resend-confirmation-email", resendRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResendConfirmationEmail_WithNonExistentEmail_ReturnsBadRequest()
    {
        // Arrange
        var resendRequest = new ResendConfirmationEmailRequestDto
        {
            Email = "nonexistent@test.com"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/resend-confirmation-email", resendRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
        await Client.PostAsJsonAsync("/api/account/register", registerRequest);

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

        var loginResponse = await Client.PostAsJsonAsync("/api/account/login", new LoginRequestDto
        {
            Email = "revoketoken@test.com",
            Password = "Password123!",
            RememberMe = false
        });
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var refreshToken = loginBody.GetProperty("refreshToken").GetString();

        var authenticatedClient = CreateAuthenticatedClient(userId!, Guid.Empty, "User", "revoketoken@test.com");

        var revokeRequest = new RevokeTokenRequestDto
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
        // Arrange
        await ClearUsersOnlyAsync();
        
        var branchId = Guid.NewGuid();
        var directorId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(directorId, branchId, "Director", "director@test.com");

        var request = new CreateStaffUserRequestDto
        {
            Email = "newstaff@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "New",
            LastName = "Staff",
            Phone = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            GenderId = Guid.NewGuid(),
            Role = "Supervisor",
            BranchId = branchId
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/create-staff-user", request);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"CreateStaffUser Response: {content}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateStaffUser_AsUser_ReturnsForbidden()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var authenticatedClient = CreateAuthenticatedClient(userId, branchId, "User", "user@test.com");

        var request = new CreateStaffUserRequestDto
        {
            Email = "stafftest@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Test",
            LastName = "Staff",
            Phone = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            GenderId = Guid.NewGuid(),
            Role = "Supervisor",
            BranchId = branchId
        };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/account/create-staff-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateStaffUser_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var request = new CreateStaffUserRequestDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Test",
            LastName = "User",
            Phone = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            GenderId = Guid.NewGuid(),
            Role = "Supervisor",
            BranchId = Guid.NewGuid()
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/account/create-staff-user", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get User Tests

    [Fact]
    public async Task GetUserById_WithValidId_ReturnsUser()
    {
        // Arrange
        await ClearUsersOnlyAsync();
        
        var registerRequest = new RegisterRequestDto
        {
            Email = "getuser@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var regResponse = await Client.PostAsJsonAsync("/api/account/register", registerRequest);
        var regBody = await regResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = regBody.GetProperty("applicationUserId").GetString();

        var authenticatedClient = CreateAuthenticatedClient(userId!, Guid.Empty, "User", "getuser@test.com");

        // Act
        var response = await authenticatedClient.GetAsync($"/api/account/user/{userId}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("getuser@test.com");
    }

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
