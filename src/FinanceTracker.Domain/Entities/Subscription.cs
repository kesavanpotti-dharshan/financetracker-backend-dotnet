using FinanceTracker.Domain.Enums;
namespace FinanceTracker.Domain.Entities;

public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public BillingCycle BillingCycle { get; set; }
    public DateOnly StartDate { get; set; } // first billing date; next due date is computed from this + cycle
    public string? Category { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
