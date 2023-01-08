using Lineweights.Core.Documents;

namespace Lineweights.IFC;

public class IfcAssetFactory : IAssetFactory
{
    /// <inheritdoc/>
    public IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context)
    {
        ModelToIfcAsset converter = new(context.StorageStrategy);
        return new []{ converter.Convert(context.Model) };
    }
}
