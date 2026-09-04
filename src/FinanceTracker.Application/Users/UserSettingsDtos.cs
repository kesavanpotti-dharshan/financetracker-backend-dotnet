namespace FinanceTracker.Application.Users;

public record UserSettingsDto(string Email, string? PreferredSecondaryCurrency);
public record UpdateSecondaryCurrencyCommand(string? Currency);