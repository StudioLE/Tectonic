using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using StudioLE.Core.Exceptions;

namespace Lineweights.SVG.From.Elements;

/// <summary>
/// Convert either a <see cref="Sheet"/> or <see cref="View"/> to an SVG file
/// referenced as <see cref="Asset"/>.
/// </summary>
public class CanvasToSvgAsset : IConverter<Canvas, Task<Asset>>
{
    private readonly IStorageStrategy _storageStrategy;

    public CanvasToSvgAsset(IStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    /// <inheritdoc />
    public async Task<Asset> Convert(Canvas canvas)
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
        return await _storageStrategy.WriteAsync(asset, fileName, stream);
    }
}
