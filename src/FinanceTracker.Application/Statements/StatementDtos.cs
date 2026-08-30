// StatementDtos.cs
public record StatementDto(Guid Id, Guid AccountId, string Status, string? RawExtractedJson, DateTime UploadedAt, string? ErrorMessage);
public record ConfirmStatementCommand(Guid StatementId, decimal ConfirmedBalance, DateOnly AsOfDate);
public record ConfirmedBalanceDto(Guid AccountId, decimal Balance, DateOnly AsOfDate);