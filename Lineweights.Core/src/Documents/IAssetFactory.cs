namespace Lineweights.Core.Documents;

public interface IAssetFactory
{
    IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context);
}
