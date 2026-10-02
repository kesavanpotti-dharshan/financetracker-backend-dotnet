using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence;

public class SubscriptionRepository(AppDbContext context) : ISubscriptionRepository
{
    public Task<List<Subscription>> GetAllForUserAsync(Guid userId) =>
        context.Subscriptions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync();

    public Task<Subscription?> GetByIdAsync(Guid id, Guid userId) =>
        context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

    public async Task AddAsync(Subscription subscription) => await context.Subscriptions.AddAsync(subscription);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
