using FinanceTracker.Domain.Enums;
namespace FinanceTracker.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public Guid? InstitutionId { get; set; }
    public Institution? Institution { get; set; }
    public string Name { get; set; } = default!;
    public AccountType AccountType { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AccountBalance> Balances { get; set; } = [];
    public CreditCardDetails? CreditCardDetails { get; set; }
    public ICollection<Statement> Statements { get; set; } = [];
    public ICollection<Transaction> Transactions { get; set; } = [];
}