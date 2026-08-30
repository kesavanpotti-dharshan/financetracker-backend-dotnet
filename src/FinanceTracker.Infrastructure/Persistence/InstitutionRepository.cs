using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence;

public class InstitutionRepository(AppDbContext context) : IInstitutionRepository
{
    public Task<List<Institution>> GetAllAsync() => context.Institutions.OrderBy(i => i.Name).ToListAsync();
    public Task<Institution?> GetByIdAsync(Guid id) => context.Institutions.FirstOrDefaultAsync(i => i.Id == id);
    public async Task AddAsync(Institution institution) => await context.Institutions.AddAsync(institution);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}