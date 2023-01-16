using System.IO;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using Lineweights.Drawings;
using StudioLE.Core.Exceptions;
using StudioLE.Core.Results;

namespace Lineweights.SVG.From.Elements;

/// <summary>
/// Convert either a <see cref="Sheet"/> or <see cref="View"/> to an SVG file
/// referenced as <see cref="IAsset"/>.
/// </summary>
public class CanvasToSvgFile : IConverter<Canvas, Task<IResult<Uri>>>
{
    private readonly IStorageStrategy _storageStrategy;
    private readonly string _fileName;

    /// <inheritdoc cref="CanvasToSvgFile" />
    public CanvasToSvgFile(IStorageStrategy storageStrategy, string fileName)
    {
        _storageStrategy = storageStrategy;
        _fileName = fileName;
    }

    /// <inheritdoc />
    public Task<IResult<Uri>> Convert(Canvas canvas)
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
        return _storageStrategy.WriteAsync(_fileName, stream);
    }
}
