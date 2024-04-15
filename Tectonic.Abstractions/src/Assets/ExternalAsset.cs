namespace Tectonic.Assets;

/// <summary>
/// A uniquely identified document that is stored externally.
/// </summary>
/// <inheritdoc cref="IAsset"/>
public record struct ExternalAsset : IAsset
{
    /// <inheritdoc/>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// The absolute path to the document, including the file name and extension.
    /// </summary>
    /// <remarks>
    /// The format of the path is dependent on the storage implementation.
    /// </remarks>
    public string AbsolutePath { get; init; } = string.Empty;

    /// <summary>
    /// Create a new instance of <see cref="ExternalAsset"/>.
    /// </summary>
    public ExternalAsset()
    {
    }
}
