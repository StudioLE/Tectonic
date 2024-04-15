namespace Tectonic.Assets;

/// <summary>
/// A uniquely identified document that is stored externally.
/// </summary>
/// <inheritdoc cref="IAsset"/>
public readonly record struct ExternalAsset : IAsset
{
    /// <inheritdoc/>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string Name { get; init; } = string.Empty;

    /// <inheritdoc/>
    public string Description { get; init; } = string.Empty;

    /// <inheritdoc/>
    public string ContentType { get; init; } = string.Empty;

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
