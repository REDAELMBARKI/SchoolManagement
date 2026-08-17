using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SchoolManagement.Api.Controllers.Auth;
using SchoolManagement.Api.Services;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.CrossCutting.Identity.Entities;
using SchoolManagement.CrossCutting.Identity.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;
using Xunit;

namespace SchoolManagement.Tests.UnitTests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IDomainUserService> _domainUserServiceMock;
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        // Initialize all mocks
        _authServiceMock = new Mock<IAuthService>();
        _domainUserServiceMock = new Mock<IDomainUserService>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _jwtServiceMock = new Mock<IJwtService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _emailServiceMock = new Mock<IEmailService>();
        _mediatorMock = new Mock<IMediator>();
        _configurationMock = new Mock<IConfiguration>();

        // Create controller instance
        _controller = new AccountController(
            _authServiceMock.Object,
            _domainUserServiceMock.Object,
            _authorizationServiceMock.Object,
            _auditLogServiceMock.Object,
            _jwtServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _emailServiceMock.Object,
            _mediatorMock.Object,
            _configurationMock.Object
        );

        // Setup HttpContext for controller (needed for Request.Scheme, Request.Host, etc.)
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    #region Register Tests

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        // TODO: Setup mocks for successful registration
        // TODO: Create RegisterRequestDto with valid email and password
        
        var request =  new Regis
        // Act
        // TODO: Call _controller.Register(request)
        
        // Assert
        // TODO: Verify result is OkObjectResult
        // TODO: Verify success message
        // TODO: Verify _authServiceMock.CreateUserAsync was called
        // TODO: Verify _emailServiceMock.SendEmailConfirmationAsync was called
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequestWithError()
    {
        // Arrange
        // TODO: Setup _authServiceMock to throw exception for duplicate email
        // TODO: Create RegisterRequestDto with existing email
        
        // Act
        // TODO: Call _controller.Register(request)
        
        // Assert
        // TODO: Verify result is BadRequestObjectResult
        // TODO: Verify error message contains appropriate text
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange
        // TODO: Setup _authServiceMock.AuthenticateAsync to return userId
        // TODO: Setup _jwtServiceMock to return access token and refresh token
        // TODO: Create LoginRequestDto with valid credentials
        
        // Act
        // TODO: Call _controller.Login(request)
        
        // Assert
        // TODO: Verify result is OkObjectResult
        // TODO: Verify response contains AccessToken and RefreshToken
        // TODO: Verify _authServiceMock.AuthenticateAsync was called once
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        // TODO: Setup _authServiceMock.AuthenticateAsync to throw exception
        // TODO: Create LoginRequestDto with invalid credentials
        
        // Act
        // TODO: Call _controller.Login(request)
        
        // Assert
        // TODO: Verify result is UnauthorizedObjectResult
        // TODO: Verify error message is present
    }

    #endregion

    #region Email Confirmation Tests

    [Fact]
    public async Task ConfirmEmailGet_WithValidToken_RedirectsToFrontendWithSuccess()
    {
        // Arrange
        // TODO: Setup _authServiceMock.ConfirmEmailAsync to succeed
        // TODO: Setup _configurationMock to return frontend URL
        // TODO: Create valid userId and token strings
        
        // Act
        // TODO: Call _controller.ConfirmEmailGet(userId, token)
        
        // Assert
        // TODO: Verify result is RedirectResult
        // TODO: Verify URL contains "emailConfirmed=true"
        // TODO: Verify _authServiceMock.ConfirmEmailAsync was called once
    }

    [Fact]
    public async Task ConfirmEmailGet_WithInvalidToken_RedirectsToFrontendWithError()
    {
        // Arrange
        // TODO: Setup _authServiceMock.ConfirmEmailAsync to throw exception
        // TODO: Setup _configurationMock to return frontend URL
        // TODO: Create userId and invalid token
        
        // Act
        // TODO: Call _controller.ConfirmEmailGet(userId, token)
        
        // Assert
        // TODO: Verify result is RedirectResult
        // TODO: Verify URL contains "emailConfirmed=false"
        // TODO: Verify URL contains error message
    }

    [Fact]
    public async Task ResendConfirmationEmail_WithValidEmail_ReturnsOkWithMessage()
    {
        // Arrange
        // TODO: Setup _authServiceMock.GetUserIdByEmailAsync to return userId
        // TODO: Setup _authServiceMock.GetApplicationUserAsync to return user with EmailConfirmed=false
        // TODO: Setup _authServiceMock.GenerateEmailConfirmationTokenAsync to return token
        // TODO: Create ResendConfirmationEmailRequestDto
        
        // Act
        // TODO: Call _controller.ResendConfirmationEmail(request)
        
        // Assert
        // TODO: Verify result is OkObjectResult
        // TODO: Verify success message
        // TODO: Verify _emailServiceMock.SendEmailConfirmationAsync was called
    }

    [Fact]
    public async Task ResendConfirmationEmail_WithAlreadyConfirmedEmail_ReturnsBadRequest()
    {
        // Arrange
        // TODO: Setup _authServiceMock.GetUserIdByEmailAsync to return userId
        // TODO: Setup _authServiceMock.GetApplicationUserAsync to return user with EmailConfirmed=true
        // TODO: Create ResendConfirmationEmailRequestDto
        
        // Act
        // TODO: Call _controller.ResendConfirmationEmail(request)
        
        // Assert
        // TODO: Verify result is BadRequestObjectResult
        // TODO: Verify error message indicates email already confirmed
    }

    #endregion

    #region Password Management Tests

    [Fact]
    public async Task ChangePassword_WithValidCurrentPassword_ReturnsOk()
    {
        // Arrange
        // TODO: Setup authorization to succeed
        // TODO: Setup _authServiceMock.ChangePasswordAsync to succeed
        // TODO: Create ChangePasswordRequestDto with valid passwords
        
        // Act
        // TODO: Call _controller.ChangePassword(request)
        
        // Assert
        // TODO: Verify result is OkObjectResult
        // TODO: Verify success message
        // TODO: Verify _auditLogServiceMock.StoreAsync was called for password change
    }

    [Fact]
    public async Task ForgotPassword_WithValidEmail_ReturnsOkRegardlessOfUserExistence()
    {
        // Arrange
        // TODO: Setup _authServiceMock.GetUserIdByEmailAsync to return userId (or null)
        // TODO: Setup _authServiceMock.GeneratePasswordResetTokenAsync if user exists
        // TODO: Create ForgotPasswordRequestDto
        
        // Act
        // TODO: Call _controller.ForgotPassword(request)
        
        // Assert
        // TODO: Verify result is OkObjectResult
        // TODO: Verify generic message (security - no email enumeration)
        // TODO: Verify _emailServiceMock was called only if user exists
    }

    #endregion
}
