using Lineweights.Core.Assets;

namespace Geometrician.Components.Visualization;

public class Outcome : IAsset
{
    /// <inheritdoc />
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc />
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc />
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Date and time stamp when the outcome was originally created.
    /// </summary>
    public DateTime CreationTime { get; set; } = DateTime.Now;

    public IReadOnlyCollection<IAssetFactory<IAsset>> Assets { get; set; } = Array.Empty<IAssetFactory<IAsset>>();
}
