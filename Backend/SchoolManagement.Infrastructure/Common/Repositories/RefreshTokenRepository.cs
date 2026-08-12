using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(string applicationUserId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.ApplicationUserId == applicationUserId && 
                        rt.RevokedAt == null && 
                        rt.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<RefreshToken>> GetAllByUserIdAsync(string applicationUserId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.ApplicationUserId == applicationUserId)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    public async Task RevokeAllUserTokensAsync(string applicationUserId, string revokedByIp, string reason)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.ApplicationUserId == applicationUserId && 
                        rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.Revoke(revokedByIp, reason);
        }

        await _context.SaveChangesAsync();
    }
}
