using System.IO;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using StudioLE.Results;
using StudioLE.Extensions.System.IO;
using StudioLE.Storage;

namespace Cascade.Assets.Storage;

/// <summary>
/// A strategy to store files in Azure Blob Storage.
/// </summary>
public class BlobStorageStrategy : IStorageStrategy
{
    private const string BlobConnectionString = "UseDevelopmentStorage=true";
    private const string BlobContainer = "assets";
    private static readonly BlobClientOptions _blobOptions = new()
    {
        Retry =
        {
            MaxRetries = 0,
            NetworkTimeout = TimeSpan.FromMilliseconds(1000),
            Delay = TimeSpan.Zero,
            MaxDelay = TimeSpan.Zero,
            Mode = RetryMode.Fixed
        }
    };
    private readonly BlobContainerClient _container = new(BlobConnectionString, BlobContainer, _blobOptions);

    /// <inheritdoc/>
    public async Task<IResult<Uri>> WriteAsync(string fileName, Stream stream)
    {
        try
        {
            BlobClient blob = _container.GetBlobClient(fileName);
            BlobHttpHeaders headers = new()
            {
                ContentType = fileName.GetContentTypeByExtension() ?? "application/octet-stream"
            };
            await blob.UploadAsync(stream, headers);
            stream.Close();
            stream.Dispose();
            return new Success<Uri>(blob.Uri);
        }
        catch (Exception e)
        {
            return new Failure<Uri>(e);
        }
    }
}
