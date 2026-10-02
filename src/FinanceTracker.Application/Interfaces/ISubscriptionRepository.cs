using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

public interface ISubscriptionRepository
{
    Task<List<Subscription>> GetAllForUserAsync(Guid userId);
    Task<Subscription?> GetByIdAsync(Guid id, Guid userId);
    Task AddAsync(Subscription subscription);
    Task SaveChangesAsync();
}
