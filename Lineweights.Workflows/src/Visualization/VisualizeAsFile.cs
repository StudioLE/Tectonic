using System.Diagnostics;
using System.IO;
using Lineweights.Workflows.Assets;

namespace Lineweights.Workflows.Visualization;

/// <summary>
/// Visualize a <see cref="Model"/> as local files.
/// Use an <see cref="AssetBuilder"/> to convert the <see cref="Model"/> to individual <see cref="Asset"/>.
/// Save the assets to local file storage and then open them in the default programs of the operating system.
/// </summary>
public sealed class VisualizeAsFile : IVisualizationStrategy
{
    private readonly IStorageStrategy _storageStrategy = new FileStorageStrategy();

    /// <summary>
    /// Should the file be opened.
    /// </summary>
    public bool IsOpenEnabled { get; set; } = true;

    /// <summary>
    /// The builder that determines what
    /// </summary>
    public Func<IStorageStrategy, Model, DocumentInformation, AssetBuilder> Builder { get; set; } = AssetBuilder.Default;

    /// <inheritdoc cref="VisualizeAsFile"/>
    public VisualizeAsFile(params string[] fileExtensions)
    {
        if (!fileExtensions.Any())
            return;
        Builder = (storageStrategy, model, doc) =>
        {
            AssetBuilder builder = new(storageStrategy, doc);
            if (fileExtensions.Contains(".glb"))
                builder = builder.ConvertModelToGlb(model);
            if (fileExtensions.Contains(".ifc"))
                builder = builder.ConvertModelToIfc(model);
            if (fileExtensions.Contains(".json"))
                builder = builder.ConvertModelToJson(model);
            if (fileExtensions.Contains(".svg"))
                builder = builder.ExtractViewsAndConvertToSvg(model);
            return builder;
        };
    }

    /// <inheritdoc cref="VisualizeAsFile"/>
    public async Task<Asset> Execute(Model model, DocumentInformation doc)
    {
        AssetBuilder builder = Builder(_storageStrategy, model, doc);
        Asset asset = await builder.Build();
        await _storageStrategy.RecursiveWriteContentToStorage(asset);
        if (IsOpenEnabled)
            RecursiveOpen(asset);
        return asset;
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
