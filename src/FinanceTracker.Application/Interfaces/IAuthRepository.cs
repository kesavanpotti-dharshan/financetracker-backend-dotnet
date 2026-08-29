using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddUserAsync(User user);
    Task AddRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetRefreshTokenByHashAsync(string hash);
    Task SaveChangesAsync();
}