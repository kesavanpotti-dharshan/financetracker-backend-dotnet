using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Users;

public class UserSettingsHandlers(IAuthRepository repo)
{
    public async Task<UserSettingsDto?> GetAsync(Guid userId)
    {
        var user = await repo.GetByIdAsync(userId);
        return user is null ? null : new UserSettingsDto(user.Email, user.PreferredSecondaryCurrency);
    }

    public async Task<UserSettingsDto?> UpdateSecondaryCurrencyAsync(Guid userId, UpdateSecondaryCurrencyCommand cmd)
    {
        var user = await repo.GetByIdAsync(userId);
        if (user is null) return null;

        user.PreferredSecondaryCurrency = string.IsNullOrWhiteSpace(cmd.Currency) ? null : cmd.Currency.ToUpperInvariant();
        await repo.SaveChangesAsync();
        return new UserSettingsDto(user.Email, user.PreferredSecondaryCurrency);
    }
}