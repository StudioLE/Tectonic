using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.SVG.From.Elements;

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
        CanvasToSvgAsset converter = new(storageStrategy);
        return await converter.Convert(canvas);
    }
}
