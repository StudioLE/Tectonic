using Geometrician.Core.Assets;
using Geometrician.Core.Converters;
using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Geometrician.Workflows.Assets;

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
