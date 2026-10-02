using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Subscriptions;

public class SubscriptionHandlers(ISubscriptionRepository repo)
{
    public async Task<List<SubscriptionDto>> GetAllAsync(Guid userId)
    {
        var subscriptions = await repo.GetAllForUserAsync(userId);
        return subscriptions.Select(ToDto).ToList();
    }

    public async Task<SubscriptionDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var subscription = await repo.GetByIdAsync(id, userId);
        return subscription is null ? null : ToDto(subscription);
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionCommand cmd, Guid userId)
    {
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = cmd.Name,
            Amount = cmd.Amount,
            Currency = cmd.Currency,
            BillingCycle = Enum.Parse<BillingCycle>(cmd.BillingCycle),
            StartDate = cmd.StartDate,
            Category = cmd.Category,
            Notes = cmd.Notes
        };
        await repo.AddAsync(subscription);
        await repo.SaveChangesAsync();
        return ToDto(subscription);
    }

    public async Task<SubscriptionDto?> UpdateAsync(UpdateSubscriptionCommand cmd, Guid userId)
    {
        var subscription = await repo.GetByIdAsync(cmd.Id, userId);
        if (subscription is null) return null;

        subscription.Name = cmd.Name;
        subscription.Amount = cmd.Amount;
        subscription.Currency = cmd.Currency;
        subscription.BillingCycle = Enum.Parse<BillingCycle>(cmd.BillingCycle);
        subscription.StartDate = cmd.StartDate;
        subscription.Category = cmd.Category;
        subscription.Notes = cmd.Notes;
        await repo.SaveChangesAsync();
        return ToDto(subscription);
    }

    public async Task<bool> CancelAsync(Guid id, Guid userId)
    {
        var subscription = await repo.GetByIdAsync(id, userId);
        if (subscription is null) return false;

        subscription.IsActive = false; // soft delete, matches Account.ArchiveAsync
        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<List<SubscriptionDto>> GetUpcomingAsync(Guid userId, int days = 30)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var horizon = today.AddDays(days);
        var subscriptions = await repo.GetAllForUserAsync(userId);

        return subscriptions
            .Select(ToDto)
            .Where(d => d.NextBillingDate <= horizon)
            .OrderBy(d => d.NextBillingDate)
            .ToList();
    }

    public async Task<SubscriptionSummaryDto> GetSummaryAsync(Guid userId)
    {
        var subscriptions = await repo.GetAllForUserAsync(userId);
        var totals = subscriptions
            .GroupBy(s => s.Currency)
            .Select(g => new CurrencyTotalDto(g.Key, g.Sum(s => ComputeMonthlyEquivalent(s.Amount, s.BillingCycle))))
            .OrderBy(t => t.Currency)
            .ToList();

        return new SubscriptionSummaryDto(subscriptions.Count, totals);
    }

    private static DateOnly ComputeNextBillingDate(DateOnly start, BillingCycle cycle, DateOnly today)
    {
        var next = start;
        while (next < today)
        {
            next = cycle switch
            {
                BillingCycle.Weekly => next.AddDays(7),
                BillingCycle.Monthly => next.AddMonths(1),
                BillingCycle.Quarterly => next.AddMonths(3),
                BillingCycle.Yearly => next.AddYears(1),
                _ => throw new ArgumentOutOfRangeException(nameof(cycle))
            };
        }
        return next;
    }

    private static decimal ComputeMonthlyEquivalent(decimal amount, BillingCycle cycle) => cycle switch
    {
        BillingCycle.Weekly => amount * 52m / 12m,
        BillingCycle.Monthly => amount,
        BillingCycle.Quarterly => amount / 3m,
        BillingCycle.Yearly => amount / 12m,
        _ => throw new ArgumentOutOfRangeException(nameof(cycle))
    };

    private static SubscriptionDto ToDto(Subscription s)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return new SubscriptionDto(
            s.Id, s.Name, s.Amount, s.Currency, s.BillingCycle.ToString(),
            s.StartDate, ComputeNextBillingDate(s.StartDate, s.BillingCycle, today),
            ComputeMonthlyEquivalent(s.Amount, s.BillingCycle), s.Category, s.Notes, s.IsActive);
    }
}
