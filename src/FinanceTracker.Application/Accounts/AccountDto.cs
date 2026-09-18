// AccountDto.cs
namespace FinanceTracker.Application.Accounts;

// AccountDtos.cs
public record AccountDto(
    Guid Id, string Name, string AccountType, Guid? InstitutionId, string? InstitutionName,
    string Currency, decimal CurrentBalance, DateOnly? BalanceAsOfDate, bool IsActive);

public record CreateAccountCommand(string Name, string AccountType, Guid? InstitutionId, string Currency);
// Application/Accounts/AccountDtos.cs
public record UpdateAccountCommand(Guid Id, string Name, string Currency, Guid? InstitutionId);
public record UpdateBalanceCommand(Guid AccountId, decimal Balance, DateOnly AsOfDate);
public record ArchiveAccountCommand(Guid Id);