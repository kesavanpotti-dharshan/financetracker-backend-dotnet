namespace FinanceTracker.Application.Subscriptions;

public record SubscriptionDto(
    Guid Id, string Name, decimal Amount, string Currency, string BillingCycle,
    DateOnly StartDate, DateOnly NextBillingDate, decimal MonthlyEquivalentAmount,
    string? Category, string? Notes, bool IsActive);

public record CreateSubscriptionCommand(string Name, decimal Amount, string Currency, string BillingCycle, DateOnly StartDate, string? Category, string? Notes);
public record UpdateSubscriptionCommand(Guid Id, string Name, decimal Amount, string Currency, string BillingCycle, DateOnly StartDate, string? Category, string? Notes);
public record CurrencyTotalDto(string Currency, decimal TotalMonthlyCost);
public record SubscriptionSummaryDto(int ActiveCount, List<CurrencyTotalDto> MonthlyTotalsByCurrency);
