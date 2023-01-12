using Lineweights.Core.Converters;
using Lineweights.Core.Documents;
using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Lineweights.Workflows.Documents;

public class GlbAssetFactory : ExternalAssetFactoryBase<Model>
{
    /// <inheritdoc />
    protected override IConverter<Model, Task<IResult<Uri>>> Converter { get; }

    /// <inheritdoc cref="GlbAssetFactory"/>
    public GlbAssetFactory(IStorageStrategy storageStrategy)
    {
        Converter = new ModelToGlbFile(storageStrategy, Asset.Id + ".glb");
        Asset.Name = "GlTF of Model";
        Asset.ContentType = "model/gltf-binary";
    }
}
