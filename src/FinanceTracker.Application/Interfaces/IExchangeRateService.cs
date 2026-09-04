namespace FinanceTracker.Application.Interfaces;

public interface IExchangeRateService
{
    Task<decimal?> GetRateAsync(string fromCurrency, string toCurrency);
}