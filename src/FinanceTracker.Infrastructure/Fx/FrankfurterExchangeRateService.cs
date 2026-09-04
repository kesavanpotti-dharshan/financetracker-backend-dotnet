using System.Text.Json;
using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Infrastructure.Fx;

public class FrankfurterExchangeRateService(HttpClient httpClient) : IExchangeRateService
{
    public async Task<decimal?> GetRateAsync(string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency) return 1m;

        var url = $"https://api.frankfurter.app/latest?from={fromCurrency}&to={toCurrency}";
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("rates", out var rates)) return null;
        if (!rates.TryGetProperty(toCurrency, out var rateElement)) return null;

        return rateElement.GetDecimal();
    }
}