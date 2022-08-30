using System.IO;
using System.Text;
using Ardalis.Result;
using StudioLE.Core.System.IO;

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

    /// <summary>
    /// Write any assigned <see cref="Asset.Content"/> to the <see cref="IStorageStrategy"/> and set the
    /// resulting <see cref="Uri"/> on the <see cref="Asset"/>.
    /// Recursively do the same to all <see cref="Asset.Children"/>.
    /// </summary>
    public static async Task RecursiveWriteContentToStorage(this IStorageStrategy @this, Asset asset)
    {
        if (asset.Content is not null)
        {
            string fileName = asset.GetFileNameById();
            byte[] byteArray = Encoding.ASCII.GetBytes(asset.Content);
            MemoryStream stream = new(byteArray);
            _ = await @this.WriteAsync(asset, fileName, stream);
        }
        foreach (Asset child in asset.Children)
            await @this.RecursiveWriteContentToStorage(child);
    }

    /// <summary>
    /// Write any local files to the <see cref="IStorageStrategy"/> and set the
    /// resulting <see cref="Uri"/> on the <see cref="Asset"/>.
    /// Recursively do the same to all <see cref="Asset.Children"/>.
    /// </summary>
    public static async Task RecursiveWriteLocalFilesToStorage(this IStorageStrategy @this, Asset asset)
    {
        if (asset.Info.Location is Uri uri && uri.IsFile)
        {
            string fileName = asset.GetFileNameById();
            Stream stream =  File.OpenRead(uri.AbsolutePath);
            _ = await @this.WriteAsync(asset, fileName, stream);
        }
        foreach (Asset child in asset.Children)
            await @this.RecursiveWriteLocalFilesToStorage(child);
    }

    /// <summary>
    /// Write any local files to the <see cref="IStorageStrategy"/> and set the
    /// resulting <see cref="Uri"/> on the <see cref="Asset"/>.
    /// Recursively do the same to all <see cref="Asset.Children"/>.
    /// </summary>
    private static string GetFileNameById(this Asset asset)
    {
        return asset.Info.Id + (asset.ContentType.GetExtensionByContentType() ?? ".txt");
    }
}
