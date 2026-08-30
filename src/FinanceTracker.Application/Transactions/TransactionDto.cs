namespace FinanceTracker.Application.Transactions;

public record TransactionDto(Guid Id, Guid AccountId, DateOnly Date, string Description, decimal Amount, string? Category);
public record CreateTransactionCommand(Guid AccountId, DateOnly Date, string Description, decimal Amount, string? Category);
public record UpdateTransactionCommand(Guid Id, DateOnly Date, string Description, decimal Amount, string? Category);