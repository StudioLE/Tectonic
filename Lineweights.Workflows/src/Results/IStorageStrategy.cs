using System.IO;

namespace Lineweights.Workflows.Results;

/// <summary>
/// A strategy to store files.
/// </summary>
public interface IStorageStrategy
{
    /// <summary>
    /// Upload asynchronously to blob storage via a stream.
    /// </summary>
    Task<Result> WriteAsync(
        DocumentInformation metadata,
        string fileExtension,
        string? mimeType,
        Func<Result, Stream> source);
}
