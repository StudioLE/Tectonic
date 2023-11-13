using Elements;
using Geometrician.Assets;
using Geometrician.Serialization.Json;
using StudioLE.Patterns;
using StudioLE.Results;

namespace Cascade.Assets.Factories;

public class JsonAssetFactory : InternalAssetFactoryBase<Model>
{
    /// <inheritdoc/>
    protected override IConverter<Model, Task<IResult<string>>> Converter { get; } = new ModelToJson();

    /// <inheritdoc cref="JsonAssetFactory"/>
    public JsonAssetFactory()
    {
        Asset.Name = "JSON of Model";
        Asset.ContentType = "application/json";
    }
}
