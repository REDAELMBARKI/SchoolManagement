using SchoolManagement.Domain.Common.Entities;
using System.Security.Claims;

namespace SchoolManagement.CrossCutting.Identity.Interfaces;

public interface IAuthService
{
    // User Creation & Authentication
    Task<string> CreateUserAsync(string email, string password, string role);
    Task<string> AuthenticateAsync(string email, string password, bool rememberMe = false);
    
    // Role Management
    Task AssignRoleAsync(string applicationUserId, string role);
    Task ChangeRoleAsync(string applicationUserId, string oldRole, string newRole);
    Task<List<string>> GetUserRolesAsync(string applicationUserId);
    
    // Claims Management
    Task AddClaimAsync(string applicationUserId, string claimType, string claimValue);
    Task RemoveClaimAsync(string applicationUserId, string claimType);
    Task<List<ClaimDto>> GetUserClaimsAsync(string applicationUserId);
    
    // Password Management
    Task ChangePasswordAsync(string applicationUserId, string currentPassword, string newPassword);
    Task ResetPasswordAsync(string applicationUserId, string newPassword);
    Task<string> GeneratePasswordResetTokenAsync(string applicationUserId);
    Task ResetPasswordWithTokenAsync(string applicationUserId, string token, string newPassword);
    
    // Email Confirmation
    Task<string> GenerateEmailConfirmationTokenAsync(string applicationUserId);
    Task ConfirmEmailAsync(string applicationUserId, string token);
    
    // User Queries
    Task<bool> UserExistsAsync(string email);
    Task<string?> GetUserIdByEmailAsync(string email);
    Task<ApplicationUser> GetApplicationUserAsync(string applicationUserId);
}

public class ClaimDto
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
