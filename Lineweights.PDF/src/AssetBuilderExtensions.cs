using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.PDF.From.Elements;
using StudioLE.Core.Exceptions;

namespace Lineweights.PDF;

/// <summary>
/// Methods to add PDF to <see cref="IAssetBuilder"/>.
/// </summary>
public static class AssetBuilderExtensions
{
    /// <inheritdoc cref="Asset"/>
    public static IAssetBuilder ExtractSheetsAndConvertToPdf(this IAssetBuilder @this)
    {
        IAssetBuilder.Steps steps = (model, storageStrategy) => model
            .AllElementsOfType<Sheet>()
            .Select(canvas => ConvertCanvasToPdf(canvas, storageStrategy));
        @this.AddSteps(steps);
        return @this;
    }

    /// <inheritdoc cref="Asset"/>
    public static IAssetBuilder ExtractViewsAndConvertToPdf(this IAssetBuilder @this)
    {
        IAssetBuilder.Steps steps = (model, storageStrategy) => model
            .AllElementsOfType<View>()
            .Select(canvas => ConvertCanvasToPdf(canvas, storageStrategy));
        @this.AddSteps(steps);
        return @this;
    }

    /// <inheritdoc cref="Asset"/>
    private static async Task<Asset> ConvertCanvasToPdf(Canvas canvas, IStorageStrategy storageStrategy)
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
        return await storageStrategy.WriteAsync(asset, fileName, stream);
    }
}
