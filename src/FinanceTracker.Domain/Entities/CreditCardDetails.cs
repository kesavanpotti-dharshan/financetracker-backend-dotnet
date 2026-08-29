namespace FinanceTracker.Domain.Entities;

public class CreditCardDetails
{
    public Guid AccountId { get; set; } // PK + FK (1:1)
    public Account Account { get; set; } = default!;
    public decimal CreditLimit { get; set; }
    public int StatementDay { get; set; } // day-of-month
    public int DueDay { get; set; }
    public decimal? MinPayment { get; set; }
    public decimal? InterestRate { get; set; }
}
