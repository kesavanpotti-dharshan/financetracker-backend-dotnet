using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    (string rawToken, string hash, DateTime expiresAt) GenerateRefreshToken();
    string HashToken(string rawToken); // used to look up an incoming refresh token by its hash
}