using System.Text;
using System.Text.Json;
using FinanceTracker.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FinanceTracker.Infrastructure.Ai;

public class GeminiStatementParser(IConfiguration config, HttpClient httpClient) : IStatementParser
{
    private const string ExtractionPrompt = """
        You are a financial statement parser. Extract the following from this bank/credit card statement PDF and return ONLY valid JSON, no markdown fences, no explanation:
        {
          "statementDate": "YYYY-MM-DD or null",
          "dueDate": "YYYY-MM-DD or null",
          "closingBalance": number or null,
          "creditLimit": number or null,
          "minPayment": number or null,
          "accountLast4": "string or null",
          "transactions": [
            { "date": "YYYY-MM-DD", "description": "string", "amount": number }
          ]
        }
        If a field cannot be determined, use null. Amounts should be positive numbers (use description/sign context, not negative amounts, to indicate direction).
        """;

    public async Task<string> ExtractAsync(byte[] pdfBytes)
    {
        var apiKey = config["Gemini:ApiKey"];
        var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new { text = ExtractionPrompt },
                        new { inline_data = new { mime_type = "application/pdf", data = Convert.ToBase64String(pdfBytes) } }
                    }
                }
            },
            generationConfig = new { response_mime_type = "application/json" }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("x-goog-api-key", apiKey);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseJson);
        var extractedText = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return extractedText ?? "{}";
    }
}