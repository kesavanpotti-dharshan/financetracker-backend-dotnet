using FinanceTracker.Domain.Entities;

public interface IStatementRepository
{
    Task AddAsync(Statement statement);
    Task<Statement?> GetByIdForUserAsync(Guid statementId, Guid userId);
    Task SaveChangesAsync();
}