using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;
using SchoolManagement.CrossCutting.Identity.Interfaces;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers.Auth;

[ApiController]
[Route("api/account")]

public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IDomainUserService _domainUserService;
    private readonly IAuthorizationService _authorizationService;

    public AccountController(IAuthService authService, IDomainUserService domainUserService , IAuthorizationService authorizationService)
    {
        _authService = authService;
        _authorizationService = authorizationService;
        _domainUserService = domainUserService;
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

    // POST /api/account/login - Authenticate user
    [HttpPost("login")]
    [AllowAnonymous]
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

            // Edge Case 1: User cannot change their own role (privilege escalation risk)
            if (currentUserId == applicationUserId)
            {
                return BadRequest(new { error = "You cannot change your own role." });
            }

            var oldRoles = await _authService.GetUserRolesAsync(applicationUserId);
            var oldRole = oldRoles.FirstOrDefault() ?? string.Empty;

            // Authorization Check 1: Can manage the target user's CURRENT role?
            var oldRoleCheck = await _authorizationService.AuthorizeAsync(
                User, 
                oldRole, // Check if you have authority over their current role
                "CanManageRole" // Clear policy name!
            );

            if (!oldRoleCheck.Succeeded)
            {
                return Forbid(); // 403 - You cannot change roles of users with this role
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
                return Forbid(); // 403 - You can only change roles in your own branch
            }

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
}
