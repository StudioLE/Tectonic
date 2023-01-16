using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using StudioLE.Core.Results;

namespace Geometrician.Core.Assets;

/// <summary>
/// Create an
/// </summary>
public class AssetFactory : IAssetFactory<IAsset, IAsset>
{
    private readonly IStorageStrategy _storageStrategy;

    /// <inheritdoc/>
    public IAsset Asset { get; private set; } = new InternalAsset();

    /// <inheritdoc/>
    public IResult Result { get; private set; } = new NotExecuted();


    /// <inheritdoc cref="GlbAssetFactory"/>
    public AssetFactory(IStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    /// <inheritdoc/>
    public void Setup(IAsset source)
    {
        Asset = source;
        if (source is not ExternalAsset asset)
        {
            Result = new Success();
            return;
        }
        if (asset.Location is null)
        {
            Result = new Failure("Location is not set.");
            return;
        }
        if (!asset.Location.IsFile)
            Result = new Success();
    }

    /// <inheritdoc/>
    public async Task Execute()
    {
        if (Asset is ExternalAsset asset)
            Result = await asset.TryWriteLocalFileToStorage(_storageStrategy);
    }
}
