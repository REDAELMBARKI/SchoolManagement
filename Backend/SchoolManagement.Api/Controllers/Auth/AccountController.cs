using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.CrossCutting.Identity.Interfaces;
using SchoolManagement.CrossCutting.Identity.Services;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.Interfaces;
using System.Security.Claims;

namespace SchoolManagement.Api.Controllers.Auth;

[ApiController]
[Route("api/account")]

public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IDomainUserService _domainUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAuditLogService _auditLogService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AccountController(
        IAuthService authService,
        IDomainUserService domainUserService,
        IAuthorizationService authorizationService,
        IAuditLogService auditLogService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService)
    {
        _authService = authService;
        _authorizationService = authorizationService;
        _domainUserService = domainUserService;
        _auditLogService = auditLogService;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }



    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            // Create ApplicationUser with basic "User" role (for students/parents)
            var applicationUserId = await _authService.CreateUserAsync(
                email: request.Email,
                password: request.Password,
                role: "User" // Default role for public registration
            );

            return Ok(new
            {
                message = "Registration successful. Please check your email to confirm your account.",
                applicationUserId = applicationUserId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpPost("create-staff-user")]
    [Authorize(Policy = "IsDirectorOrAbove")] // SuperAdmin or Director can create staff
    public async Task<IActionResult> CreateStaffUser([FromBody] CreateStaffUserRequestDto request)
    {
        try
        {
            // Validation 1: BranchId is required
            if (!request.BranchId.HasValue || request.BranchId.Value == Guid.Empty)
            {
                return BadRequest(new { error = "BranchId is required for staff user creation." });
            }

            // Validation 2: Check role hierarchy - User must have higher role than the role being created
            // This automatically prevents SuperAdmin creation because SuperAdmin is NOT in any managed roles list
            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                request.Role, // Target role being created
                "CanManageRole" // Clear policy name!
            );

            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 - You cannot create users with this role
            }

            // Validation 3: Non-SuperAdmin users can only create staff in their own branch
            var branchAuthResult = await _authorizationService.AuthorizeAsync(
                User,
                request.BranchId.Value, // Target BranchId (Guid)
                "IsSameBranch"
            );

            if (!branchAuthResult.Succeeded)
            {
                return Forbid(); // 403 - You can only create staff in your own branch
            }

            // Step 1: Create ApplicationUser with authentication
            var applicationUserId = await _authService.CreateUserAsync(
                email: request.Email,
                password: request.Password,
                role: request.Role
            );

            // Step 2: Create DomainUser with business data
            var userCommand = new DomainUserCommand
            {
                ApplicationUserId = applicationUserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                GenderId = request.GenderId,
                Role = request.Role,
                BranchId = request.BranchId.Value
            };

            var domainUser = await _domainUserService.CreateAsync(userCommand);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = domainUser.Id },
                new
                {
                    message = "Staff user created successfully",
                    user = domainUser
                });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var applicationUserId = await _authService.AuthenticateAsync(
                request.Email,
                request.Password,
                request.RememberMe
            );

            // Check if user has DomainUser profile (staff) or just ApplicationUser (public)
            DomainUserResponseDto? domainUser = null;
            try
            {
                domainUser = await _domainUserService.GetByApplicationUserIdAsync(applicationUserId);
            }
            catch
            {
                // No DomainUser - this is a public user (student/parent)
                // We'll generate token with limited claims
            }

            // Generate tokens based on user type
            string accessToken;
            if (domainUser != null)
            {
                // Staff user - full claims with BranchId
                accessToken = _jwtService.GenerateDomainUserAccessToken(
                    userId: domainUser.Id.ToString(),
                    email: domainUser.Email,
                    role: domainUser.Role,
                    branchId: domainUser.BranchId ?? Guid.Empty
                );
            }
            else
            {
                // Public user - basic claims only
                var user = await _authService.GetApplicationUserAsync(applicationUserId);
                accessToken = _jwtService.GenerateApplicationUserAccessToken(user);
            }

            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token to database
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var refreshTokenExpiration = request.RememberMe
                ? DateTime.UtcNow.AddDays(30)  // Remember Me: 30 days
                : DateTime.UtcNow.AddDays(7);   // Default: 7 days

            await _refreshTokenService.CreateRefreshTokenAsync(
                refreshToken,
                applicationUserId,
                refreshTokenExpiration,
                ipAddress
            );

            // Set refresh token as httpOnly cookie (most secure)
            SetRefreshTokenCookie(refreshToken, refreshTokenExpiration);

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,  // Also include in response for mobile apps
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = refreshTokenExpiration
            });
        }
        catch (Exception ex)
        {
            await _auditLogService.StoreAsync(
                action: AuditLog.FailedLoginAction(),
                entityName: "Authentication",
                entityId: Guid.Empty,
                branchId: AuditLog.SYSTEM_BRANCH_ID, // Global action - no specific branch
                newValues: new { Email = request.Email },
                message: $"Failed login attempt for {request.Email}",
                severity: AuditLog.SeverityWarning,
                category: AuditLog.CategorySecurity
            );

            return Unauthorized(new { error = ex.Message });
        }
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,        // Cannot be accessed by JavaScript (XSS protection)
            Expires = expires,
            SameSite = SameSiteMode.Strict,  // CSRF protection
            Secure = true,          // Only sent over HTTPS
            IsEssential = true,     // Not affected by GDPR consent
            Path = "/api/account"   // Only sent to auth endpoints
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    private string? GetRefreshTokenFromCookie()
    {
        return Request.Cookies["refreshToken"];
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/account"
        });
    }

    // POST /api/account/change-password - Change user's password
    [HttpPost("change-password")]
    [Authorize] // User must be authenticated
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            // Resource-based authorization: User can only change their own password OR SuperAdmin can change anyone's
            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                request.ApplicationUserId,
                "SelfOrSuperAdmin"
            );

            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            await _authService.ChangePasswordAsync(
                request.ApplicationUserId,
                request.CurrentPassword,
                request.NewPassword
            );

            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /api/account/forgot-password - Generate password reset token
    [HttpPost("forgot-password")]
    [AllowAnonymous] // Public endpoint - user forgot their password
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        try
        {
            var applicationUserId = await _authService.GetUserIdByEmailAsync(request.Email);
            if (applicationUserId == null)
            {
                return BadRequest(new { error = "User not found" });
            }

            var token = await _authService.GeneratePasswordResetTokenAsync(applicationUserId);

            // TODO: Send email with reset token
            return Ok(new
            {
                message = "Password reset token generated. Check your email.",
                token = token // In production, don't return token - send via email
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /api/account/reset-password - Reset password with token
    [HttpPost("reset-password")]
    [AllowAnonymous] // Public endpoint - user has reset token from email
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordWithTokenRequestDto request)
    {
        try
        {
            await _authService.ResetPasswordWithTokenAsync(
                request.ApplicationUserId,
                request.Token,
                request.NewPassword
            );

            await _auditLogService.StoreAsync(
                action: AuditLog.PasswordResetAction(),
                entityName: "Authentication",
                entityId: Guid.Empty,
                branchId: AuditLog.SYSTEM_BRANCH_ID, // Global action - no specific branch
                message: $"Password reset for user {request.ApplicationUserId}",
                severity: AuditLog.SeverityHigh,
                category: AuditLog.CategorySecurity
            );

            return Ok(new { message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDto request)
    {
        try
        {
            await _authService.ConfirmEmailAsync(request.ApplicationUserId, request.Token);
            return Ok(new { message = "Email confirmed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpPut("{username}/role")]
    [Authorize]
    public async Task<IActionResult> ChangeRole(string applicationUserId, [FromBody] ChangeRoleRequestDto request)
    {
        try
        {
            var currentUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (currentUserId == applicationUserId)
            {
                return BadRequest(new { error = "You cannot change your own role." });
            }

            var oldRoles = await _authService.GetUserRolesAsync(applicationUserId);
            var oldRole = oldRoles.FirstOrDefault() ?? string.Empty;

            var oldRoleCheck = await _authorizationService.AuthorizeAsync(
                User,
                oldRole,
                "CanManageRole"
            );

            if (!oldRoleCheck.Succeeded)
            {
                return Forbid();
            }

            // Authorization Check 2: Can assign the NEW role?
            var newRoleCheck = await _authorizationService.AuthorizeAsync(
                User,
                request.NewRole,
                "CanManageRole" // Same policy, different role
            );

            if (!newRoleCheck.Succeeded)
            {
                return Forbid(); // 403 - You cannot assign this role
            }

            // Authorization Check 3: Branch isolation (non-SuperAdmin can only manage same branch)
            // Get target user's DomainUser to check branch
            var targetDomainUser = await _domainUserService.GetByApplicationUserIdAsync(applicationUserId);

            var branchCheck = await _authorizationService.AuthorizeAsync(
                User,
                targetDomainUser.BranchId,
                "IsSameBranch"
            );

            if (!branchCheck.Succeeded)
            {
                return Forbid(); 
            }

            await _authService.ChangeRoleAsync(applicationUserId, oldRole, request.NewRole);

            await _auditLogService.StoreAsync(
                action: AuditLog.RoleChangedAction(),
                entityName: "DomainUser",
                entityId: targetDomainUser.Id,
                branchId: targetDomainUser.BranchId ?? AuditLog.SYSTEM_BRANCH_ID ,
                oldValues: new { Role = oldRole },
                newValues: new { Role = request.NewRole },
                message: $"User role changed from {oldRole} to {request.NewRole}",
                severity: AuditLog.SeverityCritical,
                category: AuditLog.CategorySecurity
            );

            return Ok(new
            {
                message = "Role changed successfully. User must re-login to get new JWT token.",
                oldRole = oldRole,
                newRole = request.NewRole
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /api/account/{applicationUserId}/claims - Add claim to user
    [HttpPost("{applicationUserId}/claims")]
    [Authorize(Policy = "IsSuperAdmin")] // Only SuperAdmin can manage claims
    public async Task<IActionResult> AddClaim(string applicationUserId, [FromBody] AddClaimRequestDto request)
    {
        try
        {
            await _authService.AddClaimAsync(applicationUserId, request.ClaimType, request.ClaimValue);

            return Ok(new
            {
                message = "Claim added successfully. User must re-login to get updated JWT token.",
                claim = new { type = request.ClaimType, value = request.ClaimValue }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE /api/account/{applicationUserId}/claims/{claimType} - Remove claim from user
    [HttpDelete("{applicationUserId}/claims/{claimType}")]
    [Authorize(Policy = "IsSuperAdmin")] // Only SuperAdmin can manage claims
    public async Task<IActionResult> RemoveClaim(string applicationUserId, string claimType)
    {
        try
        {
            await _authService.RemoveClaimAsync(applicationUserId, claimType);
            return Ok(new { message = "Claim removed successfully. User must re-login." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET /api/account/{applicationUserId}/claims - Get user's claims
    [HttpGet("{applicationUserId}/claims")]
    [Authorize] // User must be authenticated
    public async Task<IActionResult> GetUserClaims(string applicationUserId)
    {
        try
        {
            // Resource-based authorization: User can only view their own claims OR SuperAdmin can view anyone's
            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                applicationUserId,
                "SelfOrSuperAdmin"
            );

            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            var claims = await _authService.GetUserClaimsAsync(applicationUserId);
            return Ok(new { applicationUserId = applicationUserId, claims = claims });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET /api/account/{applicationUserId}/roles - Get user's roles
    [HttpGet("{applicationUserId}/roles")]
    [Authorize] // User must be authenticated
    public async Task<IActionResult> GetUserRoles(string applicationUserId)
    {
        try
        {
            // Resource-based authorization: User can only view their own roles OR SuperAdmin can view anyone's
            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                applicationUserId,
                "SelfOrSuperAdmin"
            );

            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            var roles = await _authService.GetUserRolesAsync(applicationUserId);
            return Ok(new { applicationUserId = applicationUserId, roles = roles });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Helper endpoint for DomainUser lookup (used by CreatedAtAction)
    [HttpGet("user/{id}")]
    [Authorize(Policy = "IsAdministratorOrAbove")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            var user = await _domainUserService.GetByIdAsync(id);
            var branchCheck = await _authorizationService.AuthorizeAsync(
                User,
                user.BranchId,
                "IsSameBranch"
            );

            if (!branchCheck.Succeeded)
            {
                return NotFound();
            }

            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto? request = null)
    {
        try
        {
            // Try to get refresh token from cookie first (web), then from body (mobile apps)
            var refreshToken = request?.RefreshToken ?? GetRefreshTokenFromCookie();

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { error = "Refresh token is required" });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var storedToken = await _refreshTokenService.GetActiveRefreshTokenAsync(refreshToken);

            if (storedToken == null)
            {
                return Unauthorized(new { error = "Invalid or expired refresh token" });
            }

            // Check if user has DomainUser profile (staff) or just ApplicationUser (public)
            DomainUserResponseDto? domainUser = null;
            try
            {
                domainUser = await _domainUserService.GetByApplicationUserIdAsync(storedToken.ApplicationUserId);
            }
            catch
            {
                // No DomainUser - this is a public user
            }

            // Generate new access token based on user type
            string newAccessToken;
            if (domainUser != null)
            {
                // Staff user
                newAccessToken = _jwtService.GenerateDomainUserAccessToken(
                    userId: domainUser.Id.ToString(),
                    email: domainUser.Email,
                    role: domainUser.Role,
                    branchId: domainUser.BranchId ?? Guid.Empty
                );
            }
            else
            {
                // Public user
                var user = await _authService.GetApplicationUserAsync(storedToken.ApplicationUserId);
                newAccessToken = _jwtService.GenerateApplicationUserAccessToken(user);
            }

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            // Rotate the token (revoke old, create new)
            await _refreshTokenService.RotateRefreshTokenAsync(
                refreshToken,
                newRefreshToken,
                ipAddress,
                refreshTokenExpiration
            );

            // Set new refresh token in cookie
            SetRefreshTokenCookie(newRefreshToken, refreshTokenExpiration);

            return Ok(new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = refreshTokenExpiration
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDto? request = null)
    {
        try
        {
            // Try to get refresh token from cookie first, then from body
            var refreshToken = request?.RefreshToken ?? GetRefreshTokenFromCookie();

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { error = "Refresh token is required" });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var token = await _refreshTokenService.GetActiveRefreshTokenAsync(refreshToken);

            if (token == null)
            {
                return BadRequest(new { error = "Invalid token" });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (token.ApplicationUserId != currentUserId)
            {
                return Forbid();
            }

            await _refreshTokenService.RevokeTokenAsync(refreshToken, ipAddress, "Revoked by user");

            // Delete cookie
            DeleteRefreshTokenCookie();

            return Ok(new { message = "Token revoked successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
