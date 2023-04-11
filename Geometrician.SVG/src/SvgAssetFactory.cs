using Geometrician.Core.Assets;
using Geometrician.Core.Storage;
using Geometrician.Drawings;
using Geometrician.SVG.From.Elements;
using StudioLE.Core.Results;

namespace Geometrician.SVG;

public class SvgAssetFactory<T> : ExternalAssetFactoryBase<T> where T : Canvas
{
    /// <inheritdoc/>
    protected override IConverter<T, Task<IResult<Uri>>> Converter { get; }

    /// <inheritdoc cref="SvgAssetFactory{T}"/>
    public SvgAssetFactory(IStorageStrategy storageStrategy)
    {
        Converter = new CanvasToSvgFile(storageStrategy, Asset.Id + ".svg");
        Asset.ContentType = "image/svg+xml";
    }

    /// <inheritdoc/>
    protected override void AfterSetup(T source)
    {
        Asset.Name = source.Name;
    }
}
