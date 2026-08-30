using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Transactions;

public class TransactionHandlers(ITransactionRepository repo, IAccountRepository accountRepo)
{
    public async Task<List<TransactionDto>> GetByAccountAsync(Guid accountId, Guid userId, DateOnly? from, DateOnly? to)
    {
        var transactions = await repo.GetByAccountAsync(accountId, userId, from, to);
        return transactions.Select(t => new TransactionDto(t.Id, t.AccountId, t.Date, t.Description, t.Amount, t.Category)).ToList();
    }
    public async Task<TransactionDto?> CreateAsync(CreateTransactionCommand cmd, Guid userId)
    {
        var account = await accountRepo.GetByIdAsync(cmd.AccountId, userId);
        if (account is null) return null;
        // simpler: just insert directly, ownership enforced via the query pattern below on read/delete
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = cmd.AccountId,
            Date = cmd.Date,
            Description = cmd.Description,
            Amount = cmd.Amount,
            Category = cmd.Category
            // StatementId stays null — manual entry
        };
        await repo.AddAsync(transaction);
        await repo.SaveChangesAsync();
        return new TransactionDto(transaction.Id, transaction.AccountId, transaction.Date, transaction.Description, transaction.Amount, transaction.Category);
    }

    public async Task<TransactionDto?> UpdateAsync(UpdateTransactionCommand cmd, Guid userId)
    {
        var transaction = await repo.GetByIdAsync(cmd.Id, userId);
        if (transaction is null) return null;

        transaction.Date = cmd.Date;
        transaction.Description = cmd.Description;
        transaction.Amount = cmd.Amount;
        transaction.Category = cmd.Category;
        await repo.SaveChangesAsync();
        return new TransactionDto(transaction.Id, transaction.AccountId, transaction.Date, transaction.Description, transaction.Amount, transaction.Category);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var transaction = await repo.GetByIdAsync(id, userId);
        if (transaction is null) return false;
        await repo.DeleteAsync(transaction);
        await repo.SaveChangesAsync();
        return true;
    }
}