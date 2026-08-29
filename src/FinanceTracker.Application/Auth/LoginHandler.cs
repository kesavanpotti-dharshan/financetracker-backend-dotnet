// LoginHandler.cs
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Auth;

public record LoginCommand(string Email, string Password);

public class LoginHandler(IAuthRepository repo, IPasswordHasher hasher, ITokenService tokens)
{
    public async Task<(AuthResult result, string refreshToken, DateTime refreshExpiresAt)> HandleAsync(LoginCommand cmd)
    {
        var user = await repo.GetByEmailAsync(cmd.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!hasher.Verify(cmd.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var access = tokens.GenerateAccessToken(user);
        var (raw, hash, expiresAt) = tokens.GenerateRefreshToken();
        await repo.AddRefreshTokenAsync(new RefreshToken { Id = Guid.NewGuid(), UserId = user.Id, TokenHash = hash, ExpiresAt = expiresAt });

        await repo.SaveChangesAsync();
        return (new AuthResult(access, user), raw, expiresAt);
    }
}