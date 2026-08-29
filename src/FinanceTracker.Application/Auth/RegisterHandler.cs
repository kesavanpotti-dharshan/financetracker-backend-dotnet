using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Auth;

public class RegisterHandler(IAuthRepository repo, IPasswordHasher hasher, ITokenService tokens)
{
    public async Task<(AuthResult result, string refreshToken, DateTime refreshExpiresAt)> HandleAsync(RegisterCommand cmd)
    {
        var existing = await repo.GetByEmailAsync(cmd.Email);
        if (existing is not null) throw new InvalidOperationException("Email already registered");

        var user = new User { Id = Guid.NewGuid(), Email = cmd.Email, PasswordHash = hasher.Hash(cmd.Password) };
        await repo.AddUserAsync(user);

        var access = tokens.GenerateAccessToken(user);
        var (raw, hash, expiresAt) = tokens.GenerateRefreshToken();
        await repo.AddRefreshTokenAsync(new RefreshToken { Id = Guid.NewGuid(), UserId = user.Id, TokenHash = hash, ExpiresAt = expiresAt });

        await repo.SaveChangesAsync();
        return (new AuthResult(access, user), raw, expiresAt);
    }
}