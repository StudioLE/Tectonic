using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Documents;

public class JsonAssetFactory : IAssetFactory
{
    /// <inheritdoc/>
    public IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context)
    {
        ModelToJsonAsset converter = new();
        return new [] { converter.Convert(context.Model) };
    }
}
