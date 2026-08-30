using Azure.Storage.Blobs;
using FinanceTracker.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FinanceTracker.Infrastructure.Storage;

public class AzureBlobStorage(IConfiguration config) : IFileStorage
{
    private BlobContainerClient GetContainer()
    {
        var client = new BlobServiceClient(config["Azure:BlobConnectionString"]);
        var container = client.GetBlobContainerClient(config["Azure:BlobContainerName"]);
        container.CreateIfNotExists();
        return container;
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var container = GetContainer();
        var blobName = $"{Guid.NewGuid()}-{fileName}";
        var blobClient = container.GetBlobClient(blobName);
        await blobClient.UploadAsync(fileStream, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType });
        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadAsync(string fileUrl)
    {
        var container = GetContainer();
        var blobName = new Uri(fileUrl).Segments[^1];
        var blobClient = container.GetBlobClient(blobName);
        var download = await blobClient.DownloadStreamingAsync();
        return download.Value.Content;
    }
}