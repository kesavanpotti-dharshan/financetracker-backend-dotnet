using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence;

public class AuthRepository(AppDbContext context) : IAuthRepository
{
    public Task<User?> GetByEmailAsync(string email) =>
        context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByIdAsync(Guid id) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task AddUserAsync(User user) => await context.Users.AddAsync(user);

    public async Task AddRefreshTokenAsync(RefreshToken token) => await context.RefreshTokens.AddAsync(token);

    public Task<RefreshToken?> GetRefreshTokenByHashAsync(string hash) =>
        context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.TokenHash == hash);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}