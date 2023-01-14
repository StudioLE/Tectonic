using System.Diagnostics;
using System.IO;
using Geometrician.Core.Configuration;
using Geometrician.Core.Storage;
using Geometrician.Core.Visualization;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using StudioLE.Core.Results;
using StudioLE.Core.System.IO;

namespace Lineweights.Diagnostics.Visualization;

/// <summary>
/// Visualize a <see cref="Model"/> as local files.
/// Use an <see cref="AssetFactoryResolver"/> to convert the <see cref="Model"/> to individual <see cref="IAsset"/>.
/// Save the assets to local file storage and then open them in the default programs of the operating system.
/// </summary>
public sealed class VisualizeAsFile : IVisualizationStrategy
{
    private readonly AssetFactoryResolver _resolver;
    private readonly IStorageStrategy _storageStrategy = new FileStorageStrategy();

    /// <summary>
    /// Should the file be opened.
    /// </summary>
    public bool IsOpenEnabled { get; set; } = true;

    /// <inheritdoc cref="VisualizeAsFile"/>
    public VisualizeAsFile(AssetFactoryResolver resolver)
    {
        _resolver = resolver;
    }

    /// <inheritdoc cref="VisualizeAsFile"/>
    public async Task Execute(params VisualizeRequest[] requests)
    {
        foreach (VisualizeRequest request in requests)
            await Execute(request);
    }

    /// <inheritdoc cref="VisualizeAsFile"/>
    public async Task Execute(VisualizeRequest request)
    {
        IReadOnlyCollection<IAssetFactory<IAsset>> factories = _resolver.ResolveForObjectProperties(request);

        IEnumerable<Task<ExternalAsset?>> assets = factories
            .Select(async factory =>
            {
                await factory.Execute();
                switch (factory.Asset)
                {
                    case ExternalAsset externalAsset:
                        return externalAsset;
                    case InternalAsset internalAsset:
                        string fileExtension = factory.Asset.ContentType.GetExtensionByContentType() ?? ".txt";
                        string fileName = factory.Asset.Id + fileExtension;
                        IResult<ExternalAsset> result = await internalAsset.ToExternalAsset(_storageStrategy, fileName);
                        return result is Success<ExternalAsset> success
                            ? success.Value
                            : null;
                    default:
                        return null;
                }
            });



        foreach (Task<ExternalAsset?> task in assets)
        {
            ExternalAsset? asset = await task;
            if (IsOpenEnabled
                && asset?.Location is not null
                && File.Exists(asset.Location.AbsolutePath))
                Process.Start(new ProcessStartInfo(asset.Location.AbsolutePath)
                {
                    UseShellExecute = true
                });
        }
    }
}
