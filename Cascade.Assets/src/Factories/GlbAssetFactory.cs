using Geometrician.Core.Assets;
using Geometrician.Core.Converters;
using Geometrician.Core.Storage;
using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Cascade.Assets.Factories;

public class GlbAssetFactory : ExternalAssetFactoryBase<Model>
{
    /// <inheritdoc/>
    protected override IConverter<Model, Task<IResult<Uri>>> Converter { get; }

    /// <inheritdoc cref="GlbAssetFactory"/>
    public GlbAssetFactory(IStorageStrategy storageStrategy)
    {
        Converter = new ModelToGlbFile(storageStrategy, Asset.Id + ".glb");
        Asset.Name = "GlTF of Model";
        Asset.ContentType = "model/gltf-binary";
    }
}
