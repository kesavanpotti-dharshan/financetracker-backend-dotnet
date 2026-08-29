// LogoutHandler.cs
using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Auth;

public class LogoutHandler(IAuthRepository repo, ITokenService tokens)
{
    public async Task HandleAsync(string incomingRawToken)
    {
        var hash = tokens.HashToken(incomingRawToken);
        var existing = await repo.GetRefreshTokenByHashAsync(hash);
        if (existing is not null && existing.RevokedAt is null)
        {
            existing.RevokedAt = DateTime.UtcNow;
            await repo.SaveChangesAsync();
        }
    }
}