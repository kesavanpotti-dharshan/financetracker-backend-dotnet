namespace FinanceTracker.Application.Institutions;

public record InstitutionDto(Guid Id, string Name, string? Type);
public record CreateInstitutionCommand(string Name, string? Type);