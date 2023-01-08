using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Documents;

public class GlbAssetFactory : IAssetFactory
{
    /// <inheritdoc/>
    public IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context)
    {
        ModelToGlbAsset converter = new(context.StorageStrategy);
        return new [] { converter.Convert(context.Model) };
    }
}
