using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence;

public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public Task<List<Account>> GetAllForUserAsync(Guid userId) =>
        context.Accounts
            .Where(a => a.UserId == userId && a.IsActive)
            .Include(a => a.Institution)
            .Include(a => a.CreditCardDetails)
            .Include(a => a.Balances.OrderByDescending(b => b.AsOfDate).Take(1)) // eager-load latest balance only
            .ToListAsync();

    public Task<Account?> GetByIdAsync(Guid id, Guid userId) =>
        context.Accounts
            .Include(a => a.Institution)
            .Include(a => a.CreditCardDetails)
            .Include(a => a.Balances.OrderByDescending(b => b.AsOfDate).Take(1))
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId); // ownership filter baked into the query itself

    public async Task AddAsync(Account account) => await context.Accounts.AddAsync(account);

    public async Task AddBalanceAsync(AccountBalance balance) => await context.AccountBalances.AddAsync(balance);

    public Task SaveChangesAsync() => context.SaveChangesAsync();

    public async Task<CreditCardDetails?> GetCreditCardDetailsAsync(Guid accountId, Guid userId)
    {
        var account = await context.Accounts
            .Include(a => a.CreditCardDetails)
            .Include(a => a.Balances.OrderByDescending(b => b.AsOfDate).Take(1))
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId && a.AccountType == AccountType.CreditCard);

        return account?.CreditCardDetails;
    }

    public async Task UpsertCreditCardDetailsAsync(CreditCardDetails details)
    {
        var existing = await context.CreditCardDetails.FindAsync(details.AccountId);
        if (existing is null)
        {
            await context.CreditCardDetails.AddAsync(details);
        }
        else
        {
            existing.CreditLimit = details.CreditLimit;
            existing.StatementDay = details.StatementDay;
            existing.DueDay = details.DueDay;
            existing.MinPayment = details.MinPayment;
            existing.InterestRate = details.InterestRate;
        }
    }
}