using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IRefreshTokenService
{
    /// <summary>
    /// Creates a new refresh token for the given user
    /// </summary>
    Task<RefreshToken> CreateRefreshTokenAsync(
        string token,
        string applicationUserId,
        DateTime expiresAt,
        string ipAddress);

    /// <summary>
    /// Validates and retrieves an active refresh token
    /// </summary>
    Task<RefreshToken?> GetActiveRefreshTokenAsync(string token);

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    Task RevokeTokenAsync(string token, string ipAddress, string? reason = null, string? replacedByToken = null);

    /// <summary>
    /// Rotates a refresh token (revoke old, create new)
    /// </summary>
    Task<RefreshToken> RotateRefreshTokenAsync(
        string oldToken,
        string newToken,
        string ipAddress,
        DateTime newExpiresAt);

    /// <summary>
    /// Revokes all refresh tokens for a specific user (useful for logout all devices)
    /// </summary>
    Task RevokeAllUserTokensAsync(string applicationUserId, string ipAddress, string reason);
}
