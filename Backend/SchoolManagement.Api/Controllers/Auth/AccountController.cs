using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.CrossCutting.Identity.Interfaces;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers.Auth;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IDomainUserService _domainUserService;

    public AccountController(IAuthService authService, IDomainUserService domainUserService)
    {
        _authService = authService;
        _domainUserService = domainUserService;
    }

    // POST /api/account/register - Public registration (Students/Parents)
    // Creates ApplicationUser ONLY (for login), NO DomainUser
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginRequestDto request)
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

    // POST /api/account/create-staff-user - Admin creates staff (SuperAdmin/Director only)
    // Creates ApplicationUser + DomainUser together
    [HttpPost("create-staff-user")]
    // [Authorize(Roles = "SuperAdmin,Director")] // TODO: Add when authentication is configured
    public async Task<IActionResult> CreateStaffUser([FromBody] RegisterUserRequestDto request)
    {
        try
        {
            // Validation: Prevent SuperAdmin creation via API
            if (request.Role == "SuperAdmin")
            {
                return BadRequest(new { error = "SuperAdmin cannot be created via API. Only one SuperAdmin exists (seeded in database)." });
            }

            // Validation: BranchId is required
            if (!request.BranchId.HasValue || request.BranchId.Value == Guid.Empty)
            {
                return BadRequest(new { error = "BranchId is required for staff user creation." });
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

    // POST /api/account/login - Authenticate user
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var applicationUserId = await _authService.AuthenticateAsync(request.Email, request.Password);
            
            // TODO: Generate JWT token here
            return Ok(new
            {
                message = "Login successful",
                applicationUserId = applicationUserId,
                // token = GenerateJwtToken(...)
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    // POST /api/account/change-password - Change user's password
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
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
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordWithTokenRequestDto request)
    {
        try
        {
            await _authService.ResetPasswordWithTokenAsync(
                request.ApplicationUserId,
                request.Token,
                request.NewPassword
            );

            return Ok(new { message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /api/account/confirm-email - Confirm email with token
    [HttpPost("confirm-email")]
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

    // PUT /api/account/{applicationUserId}/role - Change user's role
    [HttpPut("{applicationUserId}/role")]
    public async Task<IActionResult> ChangeRole(string applicationUserId, [FromBody] ChangeRoleRequestDto request)
    {
        try
        {
            var oldRoles = await _authService.GetUserRolesAsync(applicationUserId);
            var oldRole = oldRoles.FirstOrDefault() ?? string.Empty;

            await _authService.ChangeRoleAsync(applicationUserId, oldRole, request.NewRole);

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
    public async Task<IActionResult> GetUserClaims(string applicationUserId)
    {
        try
        {
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
    public async Task<IActionResult> GetUserRoles(string applicationUserId)
    {
        try
        {
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
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            var user = await _domainUserService.GetByIdAsync(id);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
