using FinanceTracker.Domain.Entities;

public interface IInstitutionRepository
{
    Task<List<Institution>> GetAllAsync();
    Task<Institution?> GetByIdAsync(Guid id);
    Task AddAsync(Institution institution);
    Task SaveChangesAsync();
}