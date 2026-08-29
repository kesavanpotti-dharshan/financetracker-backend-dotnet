namespace FinanceTracker.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid? StatementId { get; set; }
    public Statement? Statement { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public string? Category { get; set; }
}