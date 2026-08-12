using Microsoft.AspNetCore.Identity;
using SchoolManagement.CrossCutting.Identity.Interfaces;
using SchoolManagement.Domain.Common.Entities;
using System.Security.Claims;

namespace SchoolManagement.CrossCutting.Identity.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<string> CreateUserAsync(string email, string password, string role)
    {
        // Create ApplicationUser
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create user: {errors}");
        }

        // Assign role
        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            // Rollback: Delete user if role assignment fails
            await _userManager.DeleteAsync(user);
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to assign role: {errors}");
        }

        return user.Id;
    }

    public async Task<string> AuthenticateAsync(string email, string password, bool rememberMe = false)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new Exception("Invalid email or password.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
            var remainingMinutes = (lockoutEnd - DateTimeOffset.UtcNow)?.TotalMinutes ?? 0;
            throw new Exception($"Account is locked. Try again in {Math.Ceiling(remainingMinutes)} minutes.");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, 
            password, 
            isPersistent: false, 
            lockoutOnFailure: true
        );

        if (result.Succeeded)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
            return user.Id;
        }

        if (result.IsLockedOut)
        {
            throw new Exception("Account locked due to multiple failed login attempts. Please try again in 15 minutes.");
        }

        if (result.IsNotAllowed)
        {
            throw new Exception("Account is not allowed to sign in. Please confirm your email.");
        }

        var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
        var maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
        var attemptsRemaining = maxAttempts - failedAttempts;

        if (attemptsRemaining > 0)
        {
            throw new Exception($"Invalid email or password. {attemptsRemaining} attempts remaining before account lockout.");
        }

        throw new Exception("Invalid email or password.");
    }

    public async Task AssignRoleAsync(string applicationUserId, string role)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to assign role: {errors}");
        }
    }

    public async Task ChangeRoleAsync(string applicationUserId, string oldRole, string newRole)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        // Remove old role
        var removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
        if (!removeResult.Succeeded)
        {
            var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to remove old role: {errors}");
        }

        // Add new role
        var addResult = await _userManager.AddToRoleAsync(user, newRole);
        if (!addResult.Succeeded)
        {
            // Rollback: Re-add old role
            await _userManager.AddToRoleAsync(user, oldRole);
            var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to add new role: {errors}");
        }
    }

    public async Task<List<string>> GetUserRolesAsync(string applicationUserId)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task AddClaimAsync(string applicationUserId, string claimType, string claimValue)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var claim = new Claim(claimType, claimValue);
        var result = await _userManager.AddClaimAsync(user, claim);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to add claim: {errors}");
        }
    }

    public async Task RemoveClaimAsync(string applicationUserId, string claimType)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var claims = await _userManager.GetClaimsAsync(user);
        var claimToRemove = claims.FirstOrDefault(c => c.Type == claimType);

        if (claimToRemove == null)
        {
            throw new Exception($"Claim {claimType} not found.");
        }

        var result = await _userManager.RemoveClaimAsync(user, claimToRemove);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to remove claim: {errors}");
        }
    }

    public async Task<List<ClaimDto>> GetUserClaimsAsync(string applicationUserId)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var claims = await _userManager.GetClaimsAsync(user);
        return claims.Select(c => new ClaimDto
        {
            Type = c.Type,
            Value = c.Value
        }).ToList();
    }

    public async Task ChangePasswordAsync(string applicationUserId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to change password: {errors}");
        }
    }

    public async Task ResetPasswordAsync(string applicationUserId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        // Generate token and reset password
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to reset password: {errors}");
        }
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string applicationUserId)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task ResetPasswordWithTokenAsync(string applicationUserId, string token, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to reset password: {errors}");
        }
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string applicationUserId)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task ConfirmEmailAsync(string applicationUserId, string token)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to confirm email: {errors}");
        }
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    public async Task<string?> GetUserIdByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user?.Id;
    }

    public async Task<ApplicationUser> GetApplicationUserAsync(string applicationUserId)
    {
        var user = await _userManager.FindByIdAsync(applicationUserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        return user;
    }
}
