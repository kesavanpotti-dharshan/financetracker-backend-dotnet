using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public Task<List<Transaction>> GetByAccountAsync(Guid accountId, Guid userId, DateOnly? from, DateOnly? to)
    {
        var query = context.Transactions
            .Where(t => t.AccountId == accountId && t.Account.UserId == userId); // ownership check inline, same pattern as Accounts

        if (from is not null) query = query.Where(t => t.Date >= from);
        if (to is not null) query = query.Where(t => t.Date <= to);

        return query.OrderByDescending(t => t.Date).ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions) => await context.Transactions.AddRangeAsync(transactions);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
    public Task<Transaction?> GetByIdAsync(Guid id, Guid userId) =>
    context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.Account.UserId == userId);

    public async Task AddAsync(Transaction transaction) => await context.Transactions.AddAsync(transaction);

    public Task DeleteAsync(Transaction transaction)
    {
        context.Transactions.Remove(transaction);
        return Task.CompletedTask;
    }
}