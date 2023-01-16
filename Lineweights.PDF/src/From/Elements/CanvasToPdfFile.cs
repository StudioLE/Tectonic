using System.IO;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using Lineweights.Drawings;
using StudioLE.Core.Exceptions;
using StudioLE.Core.Results;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert either a <see cref="Sheet"/> or <see cref="View"/> to a PDF file
/// referenced as <see cref="IAsset"/>.
/// </summary>
public class CanvasToPdfFile : IConverter<Canvas, Task<IResult<Uri>>>
{
    private readonly IStorageStrategy _storageStrategy;
    private readonly string _fileName;

    public CanvasToPdfFile(IStorageStrategy storageStrategy, string fileName)
    {
        _storageStrategy = storageStrategy;
        _fileName = fileName;
    }

    /// <inheritdoc/>
    public Task<IResult<Uri>> Convert(Canvas canvas)
    {
        PdfDocument pdfDocument = canvas switch
        {
            Sheet sheet => new SheetToPdf().Convert(sheet),
            View view => new ViewToPdf().Convert(view),
            _ => throw new TypeSwitchException<Canvas>("Failed to convert canvas to pdf.", canvas)
        };
        MemoryStream stream = new();
        pdfDocument.GeneratePdf(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return _storageStrategy.WriteAsync(_fileName, stream);
    }
}
