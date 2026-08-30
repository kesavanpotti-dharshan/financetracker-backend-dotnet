using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class StatementRepository(AppDbContext context) : IStatementRepository
{
    public async Task AddAsync(Statement statement) => await context.Statements.AddAsync(statement);

    public Task<Statement?> GetByIdForUserAsync(Guid statementId, Guid userId) =>
        context.Statements.Include(s => s.Account)
            .FirstOrDefaultAsync(s => s.Id == statementId && s.Account.UserId == userId);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}