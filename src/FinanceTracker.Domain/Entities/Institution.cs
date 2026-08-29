namespace FinanceTracker.Domain.Entities;

public class Institution
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Type { get; set; }

    public ICollection<Account> Accounts { get; set; } = [];
}
