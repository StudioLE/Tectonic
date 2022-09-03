using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.SVG.From.Elements;
using StudioLE.Core.Exceptions;

namespace Lineweights.SVG;

/// <summary>
/// Methods to add SVG to <see cref="IAssetBuilder"/>.
/// </summary>
public static class AssetBuilderExtensions
{
    /// <inheritdoc cref="Asset"/>
    public static IAssetBuilder ExtractSheetsAndConvertToSvg(this IAssetBuilder @this)
    {
        IAssetBuilder.Steps steps = (model, storageStrategy) => model
            .AllElementsOfType<Sheet>()
            .Select(canvas => ConvertCanvasToSvg(canvas, storageStrategy));
        @this.AddSteps(steps);
        return @this;
    }

    /// <inheritdoc cref="Asset"/>
    public static IAssetBuilder ExtractViewsAndConvertToSvg(this IAssetBuilder @this)
    {
        IAssetBuilder.Steps steps = (model, storageStrategy) => model
            .AllElementsOfType<View>()
            .Select(canvas => ConvertCanvasToSvg(canvas, storageStrategy));
        @this.AddSteps(steps);
        return @this;
    }

    /// <inheritdoc cref="Asset"/>
    private static async Task<Asset> ConvertCanvasToSvg(Canvas canvas, IStorageStrategy storageStrategy)
    {
        SvgDocument svgDocument = canvas switch
        {
            // TODO: If new() works for View then we should amend SheetToSvg to also return the SVG element.
            Sheet sheet => new SheetToSvg().Convert(sheet),
            View view => new(new ViewToSvg().Convert(view)),
            _ => throw new TypeSwitchException<Canvas>("Failed to convert canvas to svg.", canvas)
        };
        MemoryStream stream = new();
        svgDocument.Save(stream, SaveOptions.None);
        stream.Seek(0, SeekOrigin.Begin);

        Asset asset = new()
        {
            Info = new()
            {
                Id = canvas.Id,
                Name = canvas.Name
            },
            ContentType = "image/svg+xml"
        };
        string fileName = asset.Info.Id + ".svg";
        return await storageStrategy.WriteAsync(asset, fileName, stream);
    }
}
