using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Accounts;

public class AccountHandlers(IAccountRepository repo)
{
    public async Task<List<AccountDto>> GetAllAsync(Guid userId)
    {
        var accounts = await repo.GetAllForUserAsync(userId);
        return accounts.Select(ToDto).ToList();
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var account = await repo.GetByIdAsync(id, userId);
        return account is null ? null : ToDto(account);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountCommand cmd, Guid userId)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = cmd.Name,
            AccountType = Enum.Parse<AccountType>(cmd.AccountType),
            InstitutionId = cmd.InstitutionId,
            Currency = cmd.Currency
        };
        await repo.AddAsync(account);
        await repo.SaveChangesAsync();

        var created = await repo.GetByIdAsync(account.Id, userId);
        return ToDto(created!);
    }

    public async Task<AccountDto?> UpdateAsync(UpdateAccountCommand cmd, Guid userId)
    {
        var account = await repo.GetByIdAsync(cmd.Id, userId);
        if (account is null) return null;

        account.Name = cmd.Name;
        account.Currency = cmd.Currency;
        await repo.SaveChangesAsync();
        return ToDto(account);
    }

    public async Task<bool> ArchiveAsync(Guid id, Guid userId)
    {
        var account = await repo.GetByIdAsync(id, userId);
        if (account is null) return false;

        account.IsActive = false; // soft delete, per schema design
        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<AccountDto?> UpdateBalanceAsync(UpdateBalanceCommand cmd, Guid userId)
    {
        var account = await repo.GetByIdAsync(cmd.AccountId, userId);
        if (account is null) return null;

        await repo.AddBalanceAsync(new AccountBalance
        {
            Id = Guid.NewGuid(),
            AccountId = cmd.AccountId,
            Balance = cmd.Balance,
            AsOfDate = cmd.AsOfDate,
            Source = BalanceSource.Manual
        });
        await repo.SaveChangesAsync();

        var updated = await repo.GetByIdAsync(cmd.AccountId, userId);
        return ToDto(updated!);
    }

    private static AccountDto ToDto(Account a)
    {
        var latest = a.Balances.OrderByDescending(b => b.AsOfDate).FirstOrDefault();
        return new AccountDto(
            a.Id, a.Name, a.AccountType.ToString(), a.Institution?.Name,
            a.Currency, latest?.Balance ?? 0m, latest?.AsOfDate, a.IsActive);
    }
    public async Task<CreditCardDetailsDto?> GetCreditCardDetailsAsync(Guid accountId, Guid userId)
    {
        var account = await repo.GetByIdAsync(accountId, userId);
        if (account is null || account.AccountType != AccountType.CreditCard) return null;

        var details = await repo.GetCreditCardDetailsAsync(accountId, userId);
        if (details is null) return null;

        var latestBalance = account.Balances.OrderByDescending(b => b.AsOfDate).FirstOrDefault()?.Balance ?? 0m;
        return new CreditCardDetailsDto(
            accountId, details.CreditLimit, details.StatementDay, details.DueDay,
            details.MinPayment, details.InterestRate, latestBalance, details.CreditLimit - latestBalance);
    }

    public async Task<CreditCardDetailsDto?> SetCreditCardDetailsAsync(SetCreditCardDetailsCommand cmd, Guid userId)
    {
        var account = await repo.GetByIdAsync(cmd.AccountId, userId);
        if (account is null || account.AccountType != AccountType.CreditCard) return null;

        await repo.UpsertCreditCardDetailsAsync(new CreditCardDetails
        {
            AccountId = cmd.AccountId,
            CreditLimit = cmd.CreditLimit,
            StatementDay = cmd.StatementDay,
            DueDay = cmd.DueDay,
            MinPayment = cmd.MinPayment,
            InterestRate = cmd.InterestRate
        });
        await repo.SaveChangesAsync();

        return await GetCreditCardDetailsAsync(cmd.AccountId, userId);
    }
}