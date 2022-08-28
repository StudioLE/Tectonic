using System.IO;
using Ardalis.Result;

namespace Lineweights.Workflows.Containers;

/// <summary>
/// A strategy to store files.
/// </summary>
public interface IStorageStrategy
{
    /// <summary>
    /// Write a file asynchronously via a stream.
    /// </summary>
    Task<Result<Uri>> WriteAsync(string fileName, Stream stream);
}


public static class StorageStrategyExtensions
{
    /// <summary>
    /// Write a file asynchronously via a stream.
    /// If any errors occur during writing then add them to <see cref="Container.Errors"/>.
    /// </summary>
    public static async Task<Container> WriteAsync(this IStorageStrategy storageStrategy, Container container, string fileName, Stream stream)
    {
        Result<Uri> uriResult = await storageStrategy.WriteAsync(fileName, stream);
        if (!uriResult.IsSuccess)
            container.Errors = container.Errors.Concat(uriResult.Errors).ToArray();
        container.Info.Location = uriResult;
        return container;
    }
}
