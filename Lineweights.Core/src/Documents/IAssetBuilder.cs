using System.Collections.ObjectModel;

namespace Lineweights.Core.Documents;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/builder">builder</see> to create an <see cref="Asset"/>.
/// </summary>
public interface IAssetBuilder
{
    public IStorageStrategy StorageStrategy { get; set; }

    public Collection<BuildTask> Tasks { get; }

    /// <summary>
    /// Build asset.
    /// </summary>
    public Task<Asset> Build(Model model);

    public delegate IEnumerable<Task<Asset>> BuildTask(Model model, IStorageStrategy storageStrategy);
}
