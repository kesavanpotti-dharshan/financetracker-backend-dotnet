namespace FinanceTracker.Application.Accounts;

public record CreditCardDetailsDto(
    Guid AccountId,
    decimal CreditLimit,
    int StatementDay,
    int DueDay,
    decimal? MinPayment,
    decimal? InterestRate,
    decimal CurrentBalance,
    decimal AvailableCredit);

public record SetCreditCardDetailsCommand(
    Guid AccountId,
    decimal CreditLimit,
    int StatementDay,
    int DueDay,
    decimal? MinPayment,
    decimal? InterestRate);