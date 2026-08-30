using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

public interface IAccountRepository
{
    Task<List<Account>> GetAllForUserAsync(Guid userId);
    Task<Account?> GetByIdAsync(Guid id, Guid userId);
    Task AddAsync(Account account);
    Task AddBalanceAsync(AccountBalance balance);
    Task SaveChangesAsync();
    Task<CreditCardDetails?> GetCreditCardDetailsAsync(Guid accountId, Guid userId);
    Task UpsertCreditCardDetailsAsync(CreditCardDetails details);
}