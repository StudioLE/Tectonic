using System.Diagnostics;
using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Workflows.Documents;

namespace Lineweights.Workflows.Visualization;

/// <summary>
/// Visualize a <see cref="Model"/> as local files.
/// Use an <see cref="AssetBuilder"/> to convert the <see cref="Model"/> to individual <see cref="Asset"/>.
/// Save the assets to local file storage and then open them in the default programs of the operating system.
/// </summary>
public sealed class VisualizeAsFile : IVisualizationStrategy
{
    private readonly IStorageStrategy _storageStrategy = new FileStorageStrategy();
    private readonly IAssetBuilder _builder;

    /// <summary>
    /// Should the file be opened.
    /// </summary>
    public bool IsOpenEnabled { get; set; } = true;

    /// <inheritdoc cref="VisualizeAsFile"/>
    public VisualizeAsFile(IAssetBuilder builder)
    {
        _builder = builder;
        _builder.StorageStrategy = _storageStrategy;
    }

    /// <inheritdoc cref="VisualizeAsFile"/>
    public async Task Execute(VisualizeRequest request)
    {
        request.Asset = await _builder
            .AddAssets(request.Asset)
            .Build(request.Model);
        await _storageStrategy.RecursiveWriteContentToStorage(request.Asset);
        if (IsOpenEnabled)
            RecursiveOpen(request.Asset);
    }

    private static void RecursiveOpen(Asset asset)
    {
        if (File.Exists(asset.Info.Location?.AbsolutePath))
            Process.Start(new ProcessStartInfo(asset.Info.Location!.AbsolutePath)
            {
                UseShellExecute = true
            });
        foreach (Asset child in asset.Children)
            RecursiveOpen(child);
    }
}
