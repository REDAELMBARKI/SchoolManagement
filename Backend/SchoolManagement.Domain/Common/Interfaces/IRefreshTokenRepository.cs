using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Domain.Common.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(string applicationUserId);
    Task<List<RefreshToken>> GetAllByUserIdAsync(string applicationUserId);
    Task RevokeAllUserTokensAsync(string applicationUserId, string revokedByIp, string reason);
}
