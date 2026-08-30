// AccountDto.cs
namespace FinanceTracker.Application.Accounts;

public record AccountDto(
    Guid Id,
    string Name,
    string AccountType,
    string? InstitutionName,
    string Currency,
    decimal CurrentBalance,
    DateOnly? BalanceAsOfDate,
    bool IsActive);

public record CreateAccountCommand(string Name, string AccountType, Guid? InstitutionId, string Currency);
public record UpdateAccountCommand(Guid Id, string Name, string Currency);
public record UpdateBalanceCommand(Guid AccountId, decimal Balance, DateOnly AsOfDate);
public record ArchiveAccountCommand(Guid Id);