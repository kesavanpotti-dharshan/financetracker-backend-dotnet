namespace FinanceTracker.Application.Interfaces;

public interface IFileStorage
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> DownloadAsync(string fileUrl);
}