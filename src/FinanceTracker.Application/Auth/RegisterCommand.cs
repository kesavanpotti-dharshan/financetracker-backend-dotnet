using FinanceTracker.Domain.Entities;

// RegisterCommand.cs
namespace FinanceTracker.Application.Auth;

public record RegisterCommand(string Email, string Password);
public record AuthResult(string AccessToken, User User);