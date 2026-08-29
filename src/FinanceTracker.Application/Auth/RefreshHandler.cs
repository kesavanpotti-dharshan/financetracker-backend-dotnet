// RefreshHandler.cs
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Auth;

public class RefreshHandler(IAuthRepository repo, ITokenService tokens)
{
    public async Task<(AuthResult result, string refreshToken, DateTime refreshExpiresAt)> HandleAsync(string incomingRawToken)
    {
        var hash = tokens.HashToken(incomingRawToken);
        var existing = await repo.GetRefreshTokenByHashAsync(hash)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        if (!existing.IsActive)
            throw new UnauthorizedAccessException("Refresh token expired or revoked");

        // rotate: revoke old, issue new
        var access = tokens.GenerateAccessToken(existing.User);
        var (newRaw, newHash, newExpiresAt) = tokens.GenerateRefreshToken();

        existing.RevokedAt = DateTime.UtcNow;
        existing.ReplacedByTokenHash = newHash;

        await repo.AddRefreshTokenAsync(new RefreshToken { Id = Guid.NewGuid(), UserId = existing.UserId, TokenHash = newHash, ExpiresAt = newExpiresAt });
        await repo.SaveChangesAsync();

        return (new AuthResult(access, existing.User), newRaw, newExpiresAt);
    }
}