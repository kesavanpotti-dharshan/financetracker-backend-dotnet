using FinanceTracker.Domain.Entities;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetByAccountAsync(Guid accountId, Guid userId, DateOnly? from, DateOnly? to);
    Task AddRangeAsync(IEnumerable<Transaction> transactions);
    Task SaveChangesAsync();
    Task<Transaction?> GetByIdAsync(Guid id, Guid userId);
    Task AddAsync(Transaction transaction);
    Task DeleteAsync(Transaction transaction);
}