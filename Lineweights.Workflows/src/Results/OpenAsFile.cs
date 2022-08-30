using System.Diagnostics;
using System.IO;
using System.Text;
using Lineweights.Workflows.Assets;
using StudioLE.Core.System.IO;

namespace Lineweights.Workflows.Results;

/// <summary>
/// An <see cref="IResultStrategy"/> to visualise a <see cref="Model"/> by saving it to a file and opening it locally.
/// </summary>
public sealed class OpenAsFile : IResultStrategy
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

    /// <inheritdoc cref="OpenAsFile"/>
    public OpenAsFile(params string[] fileExtensions)
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

    /// <inheritdoc cref="OpenAsFile"/>
    public async Task<Asset> Execute(Model model, DocumentInformation doc)
    {
        AssetBuilder builder = Builder(_storageStrategy, model, doc);
        Asset asset = await builder.Build();
        await RecursiveWriteContent(asset);
        if (IsOpenEnabled)
            RecursiveOpen(asset);
        return asset;
    }

    private async Task RecursiveWriteContent(Asset asset)
    {
        if (asset.Content is not null)
        {
            string fileName = asset.Info.Id + (asset.ContentType.GetExtensionByContentType() ?? ".txt");
            byte[] byteArray = Encoding.ASCII.GetBytes(asset.Content);
            MemoryStream stream = new(byteArray);
            _ = await _storageStrategy.WriteAsync(asset, fileName, stream);
        }
        foreach (Asset child in asset.Children)
            await RecursiveWriteContent(child);
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
