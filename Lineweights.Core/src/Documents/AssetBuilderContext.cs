namespace Lineweights.Core.Documents;

public interface IAssetBuilderContext
{
    public IStorageStrategy StorageStrategy { get; set; }

    public Model Model { get; set; }
}
