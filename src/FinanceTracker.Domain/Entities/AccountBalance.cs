using FinanceTracker.Domain.Enums;
namespace FinanceTracker.Domain.Entities;

public class AccountBalance
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = default!;
    public decimal Balance { get; set; }
    public DateOnly AsOfDate { get; set; }
    public BalanceSource Source { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
