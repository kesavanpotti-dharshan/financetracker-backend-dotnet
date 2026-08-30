namespace FinanceTracker.Application.Interfaces;

public interface IStatementParser
{
    Task<string> ExtractAsync(byte[] pdfBytes); // returns raw JSON string from Gemini
}