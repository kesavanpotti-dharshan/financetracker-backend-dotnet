using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Institutions;

public class InstitutionHandlers(IInstitutionRepository repo)
{
    public async Task<List<InstitutionDto>> GetAllAsync() =>
        (await repo.GetAllAsync()).Select(i => new InstitutionDto(i.Id, i.Name, i.Type)).ToList();

    public async Task<InstitutionDto> CreateAsync(CreateInstitutionCommand cmd)
    {
        var institution = new Institution { Id = Guid.NewGuid(), Name = cmd.Name, Type = cmd.Type };
        await repo.AddAsync(institution);
        await repo.SaveChangesAsync();
        return new InstitutionDto(institution.Id, institution.Name, institution.Type);
    }
}