using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using StudioLE.Core.Exceptions;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert either a <see cref="Sheet"/> or <see cref="View"/> to a PDF file
/// referenced as <see cref="Asset"/>.
/// </summary>
public class CanvasToPdfAsset : IConverter<Canvas, Task<Asset>>
{
    private readonly IStorageStrategy _storageStrategy;

    public CanvasToPdfAsset(IStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    /// <inheritdoc />
    public async Task<Asset> Convert(Canvas canvas)
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

        Asset asset = new()
        {
            Info = new()
            {
                Id = canvas.Id,
                Name = canvas.Name
            },
            ContentType = "application/pdf"
        };
        string fileName = asset.Info.Id + ".pdf";
        return await _storageStrategy.WriteAsync(asset, fileName, stream);
    }
}
