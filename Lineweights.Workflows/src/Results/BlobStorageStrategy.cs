using System.IO;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Lineweights.Workflows.Results;

/// <summary>
/// A strategy to store files in Azure Blob Storage.
/// </summary>
public class BlobStorageStrategy : IStorageStrategy
{
    // TODO: BlobStorage should be injected
    // TODO: Add a local file storage alternative strategy for OpenAsFile. Must override AsyncUpload
    private const string BlobConnectionString = "UseDevelopmentStorage=true";
    private const string BlobContainer = "dashboard";
    private static readonly BlobClientOptions _blobOptions = new()
    {
        Retry =
        {
            MaxRetries = 0,
            NetworkTimeout = TimeSpan.FromMilliseconds(1000),
            Delay = TimeSpan.Zero,
            MaxDelay = TimeSpan.Zero,
            Mode = RetryMode.Fixed
        },
    };
    private readonly BlobContainerClient _container = new(BlobConnectionString, BlobContainer, _blobOptions);

    /// <summary>
    /// Upload asynchronously to blob storage via a stream.
    /// </summary>
    public async Task<Result> WriteAsync(
        DocumentInformation doc,
        string fileExtension,
        Func<Result, Stream> source)
    {
        Result result = new()
        {
            Info = doc
        };
        try
        {
            string fileName = doc.Id + fileExtension;
            BlobClient blob = _container.GetBlobClient(fileName);
            while (blob.Exists())
            {
                fileName = Guid.NewGuid() + fileExtension;
                blob = _container.GetBlobClient(fileName);
            }

            doc.Location = blob.Uri;

            Stream stream = source.Invoke(result);
            BlobHttpHeaders headers = GetHeaders(fileExtension);
            await blob.UploadAsync(stream, headers).ConfigureAwait(false);
            stream.Close();
            stream.Dispose();
        }
        catch (Exception e)
        {
            result.Errors = new[]
            {
                "Failed to upload.",
                e.Message
            };
        }
        return result;
    }

    private static BlobHttpHeaders GetHeaders(string fileExtension)
    {
        string? contentType = fileExtension switch
        {
            ".glb" => "model/gltf-binary",
            ".ifc" => "application/x-step",
            ".json" => "application/json",
            ".svg" => "image/svg+xml",
            ".pdf" => "application/pdf",
            _ => null
        };
        return contentType is null
            ? new()
            : new() { ContentType = contentType };
    }
}
