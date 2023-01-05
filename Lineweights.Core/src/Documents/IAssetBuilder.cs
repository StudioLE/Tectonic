using System.Collections.ObjectModel;

namespace Lineweights.Core.Documents;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/builder">builder</see> to create an <see cref="Asset"/>.
/// </summary>
public interface IAssetBuilder
{
    public IStorageStrategy StorageStrategy { set; }

    public Collection<IAssetFactory> Factories { get; }

    /// <summary>
    /// Build asset.
    /// </summary>
    public Task<Asset> Build(Model model);

}
