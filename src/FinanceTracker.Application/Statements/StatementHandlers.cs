using System.Text.Json;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Statements;

public class StatementHandlers(
    IAccountRepository accountRepo,
    IStatementRepository statementRepo,
    ITransactionRepository transactionRepo,
    IFileStorage storage,
    IStatementParser parser)
{
    public async Task<StatementDto> UploadAsync(Guid accountId, Guid userId, Stream fileStream, string fileName)
    {
        var account = await accountRepo.GetByIdAsync(accountId, userId)
            ?? throw new UnauthorizedAccessException("Account not found or not owned by user");

        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        var pdfBytes = ms.ToArray();
        ms.Position = 0;

        var fileUrl = await storage.UploadAsync(ms, fileName, "application/pdf");

        var statement = new Statement
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            FileUrl = fileUrl,
            Status = StatementStatus.Pending
        };
        await statementRepo.AddAsync(statement);
        await statementRepo.SaveChangesAsync();

        // synchronous extraction — fine for weekend-scope personal use
        try
        {
            var extractedJson = await parser.ExtractAsync(pdfBytes);
            statement.RawExtractedJson = extractedJson;
            statement.Status = StatementStatus.Parsed;
        }
        catch (Exception ex)
        {
            statement.Status = StatementStatus.Failed;
            statement.ErrorMessage = ex.Message;
        }
        await statementRepo.SaveChangesAsync();

        return ToDto(statement);
    }

    public async Task<StatementDto?> GetByIdAsync(Guid statementId, Guid userId)
    {
        var statement = await statementRepo.GetByIdForUserAsync(statementId, userId);
        return statement is null ? null : ToDto(statement);
    }

    public async Task<ConfirmedBalanceDto?> ConfirmAsync(ConfirmStatementCommand cmd, Guid userId)
    {
        var statement = await statementRepo.GetByIdForUserAsync(cmd.StatementId, userId);
        if (statement is null) return null;

        var balance = new AccountBalance
        {
            Id = Guid.NewGuid(),
            AccountId = statement.AccountId,
            Balance = cmd.ConfirmedBalance,
            AsOfDate = cmd.AsOfDate,
            Source = BalanceSource.StatementImport
        };
        await accountRepo.AddBalanceAsync(balance);

        // parse transactions out of the raw extraction and insert them
        if (!string.IsNullOrWhiteSpace(statement.RawExtractedJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(statement.RawExtractedJson);
                if (doc.RootElement.TryGetProperty("transactions", out var txArray) && txArray.ValueKind == JsonValueKind.Array)
                {
                    var transactions = new List<Transaction>();
                    foreach (var tx in txArray.EnumerateArray())
                    {
                        var dateStr = tx.GetProperty("date").GetString();
                        if (!DateOnly.TryParse(dateStr, out var date)) continue; // skip malformed entries rather than fail the whole confirm

                        transactions.Add(new Transaction
                        {
                            Id = Guid.NewGuid(),
                            StatementId = statement.Id,
                            AccountId = statement.AccountId,
                            Date = date,
                            Description = tx.GetProperty("description").GetString() ?? "",
                            Amount = tx.GetProperty("amount").GetDecimal(),
                            Category = null
                        });
                    }
                    if (transactions.Count > 0)
                        await transactionRepo.AddRangeAsync(transactions);
                }
            }
            catch (JsonException)
            {
                // malformed AI output — balance still gets confirmed, just skip transaction import for this statement
            }
        }

        statement.Status = StatementStatus.Reviewed;
        await statementRepo.SaveChangesAsync(); // saves balance + transactions + status together (same DbContext)

        return new ConfirmedBalanceDto(balance.AccountId, balance.Balance, balance.AsOfDate);
    }

    private static StatementDto ToDto(Statement s) =>
        new(s.Id, s.AccountId, s.Status.ToString(), s.RawExtractedJson, s.UploadedAt, s.ErrorMessage);
}