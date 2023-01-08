using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.SVG.From.Elements;
using StudioLE.Core.Exceptions;

namespace Lineweights.SVG;

public class SvgAssetFactory<T> : IAssetFactory where T : Canvas
{
    /// <inheritdoc/>
    public IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context)
    {

        return context
            .Model
            .AllElementsOfType<T>()
            .Select(canvas => ConvertCanvasToSvg(canvas, context.StorageStrategy));
    }

    /// <inheritdoc cref="Asset"/>
    private static async Task<Asset> ConvertCanvasToSvg(Canvas canvas, IStorageStrategy storageStrategy)
    {
        SvgElement svgElement = canvas switch
        {
            Sheet sheet => new SheetToSvg().Convert(sheet),
            View view => new ViewToSvg().Convert(view),
            _ => throw new TypeSwitchException<Canvas>("Failed to convert canvas to svg.", canvas)
        };
        SvgDocument svgDocument = new(svgElement);
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
