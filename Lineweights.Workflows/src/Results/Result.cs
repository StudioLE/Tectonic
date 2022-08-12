using Lineweights.Core.Serialisation;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Results;

/// <summary>
/// The composite results of a workflow.
/// </summary>
public sealed class Result
{
    /// <summary>
    /// The document metadata.
    /// </summary>
    [JsonConverter(typeof(OverrideInheritanceConverter))]
    public DocumentInformation Metadata { get; set; } = new();

    /// <summary>
    /// The uri of any additional files.
    /// </summary>
    public ICollection<Result> Children { get; } = new List<Result>();

    /// <summary>
    /// The title of the tile.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; set; } = Array.Empty<string>();

    /// <inheritdoc cref="Result"/>
    internal Result()
    {
    }
}
