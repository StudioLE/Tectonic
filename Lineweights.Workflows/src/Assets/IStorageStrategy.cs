using System.IO;
using Ardalis.Result;

namespace Lineweights.Workflows.Assets;

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
    /// If any errors occur during writing then add them to <see cref="Asset.Errors"/>.
    /// </summary>
    public static async Task<Asset> WriteAsync(this IStorageStrategy storageStrategy, Asset asset, string fileName, Stream stream)
    {
        Result<Uri> uriResult = await storageStrategy.WriteAsync(fileName, stream);
        if (!uriResult.IsSuccess)
            asset.Errors = asset.Errors.Concat(uriResult.Errors).ToArray();
        asset.Info.Location = uriResult;
        return asset;
    }
}
