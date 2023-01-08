using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.PDF.From.Elements;

namespace Lineweights.PDF;

public class PdfAssetFactory<T> : IAssetFactory where T : Canvas
{
    /// <inheritdoc/>
    public IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context)
    {
        return context
            .Model
            .AllElementsOfType<T>()
            .Select(canvas => ConvertCanvasToPdf(canvas, context.StorageStrategy));
    }

    /// <inheritdoc cref="Asset"/>
    private static async Task<Asset> ConvertCanvasToPdf(Canvas canvas, IStorageStrategy storageStrategy)
    {
        CanvasToPdfAsset converter = new(storageStrategy);
        return await converter.Convert(canvas);
    }
}
