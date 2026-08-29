using FinanceTracker.Domain.Enums;
namespace FinanceTracker.Domain.Entities;

public class Statement
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = default!;
    public string FileUrl { get; set; } = default!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public StatementStatus Status { get; set; } = StatementStatus.Pending;
    public string? RawExtractedJson { get; set; } // mapped to jsonb
    public string? ErrorMessage { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = [];
}
