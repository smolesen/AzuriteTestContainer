using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;
using System.Text;
using Microsoft.Extensions.Logging;

namespace AzuriteTestContainer;
public class Job(IAzureClientFactory<BlobServiceClient> blobServiceClientFactory, ILogger<Job> logger)
    : IJob
{
    public async Task Archive()
    {
        logger.LogInformation("******************* Starting Archive job *******************");
        var blobServiceClient = blobServiceClientFactory.CreateClient("ArchiveStorage");
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("archive");
        logger.LogInformation("******************* AccountNName: {AccountName} ContainerName: {ContainerName} *******************", blobContainerClient.AccountName, blobContainerClient.Name);
        await blobContainerClient.CreateIfNotExistsAsync();
        var blobClient = blobContainerClient.GetBlobClient("blob.txt");
        var options = new BlobUploadOptions()
        {
            HttpHeaders = new BlobHttpHeaders() { ContentType = "text/plain" }
        };
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes("Hello World"));
        await blobClient.UploadAsync(ms, options);
        logger.LogInformation("******************* Ending Archive job *******************");
    }
}
