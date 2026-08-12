using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Application.Common.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repository;

    public RefreshTokenService(IRefreshTokenRepository repository)
    {
        _repository = repository;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(
        string token,
        string applicationUserId,
        DateTime expiresAt,
        string ipAddress)
    {
        var refreshToken = RefreshToken.Create(
            token,
            applicationUserId,
            expiresAt,
            ipAddress
        );

        return await _repository.AddAsync(refreshToken);
    }

    public async Task<RefreshToken?> GetActiveRefreshTokenAsync(string token)
    {
        var refreshToken = await _repository.GetByTokenAsync(token);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return null;
        }

        return refreshToken;
    }

    public async Task RevokeTokenAsync(
        string token,
        string ipAddress,
        string? reason = null,
        string? replacedByToken = null)
    {
        var refreshToken = await _repository.GetByTokenAsync(token);

        if (refreshToken == null)
        {
            throw new NotFoundException($"Refresh token not found");
        }

        if (!refreshToken.IsActive)
        {
            throw new DomainException("Token is already revoked or expired");
        }

        refreshToken.Revoke(ipAddress, reason, replacedByToken);
        await _repository.UpdateAsync(refreshToken);
    }

    public async Task<RefreshToken> RotateRefreshTokenAsync(
        string oldToken,
        string newToken,
        string ipAddress,
        DateTime newExpiresAt)
    {
        var oldRefreshToken = await _repository.GetByTokenAsync(oldToken);

        if (oldRefreshToken == null)
        {
            throw new NotFoundException("Refresh token not found");
        }

        if (!oldRefreshToken.IsActive)
        {
            throw new DomainException("Token is already revoked or expired");
        }

        // Revoke old token
        oldRefreshToken.Revoke(ipAddress, "Replaced by new token", newToken);
        await _repository.UpdateAsync(oldRefreshToken);

        // Create new token
        var newRefreshToken = RefreshToken.Create(
            newToken,
            oldRefreshToken.ApplicationUserId,
            newExpiresAt,
            ipAddress
        );

        return await _repository.AddAsync(newRefreshToken);
    }

    public async Task RevokeAllUserTokensAsync(string applicationUserId, string ipAddress, string reason)
    {
        var userTokens = await _repository.GetAllByUserIdAsync(applicationUserId);

        foreach (var token in userTokens.Where(t => t.IsActive))
        {
            token.Revoke(ipAddress, reason);
            await _repository.UpdateAsync(token);
        }
    }
}
