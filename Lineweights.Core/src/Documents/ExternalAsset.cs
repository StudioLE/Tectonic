using System.IO;
using StudioLE.Core.Results;

namespace Lineweights.Core.Documents;

/// <inheritdoc cref="IAsset"/>
public class ExternalAsset : Element, IAsset, ICloneable
{
    /// <inheritdoc />
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc />
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Resource identifier or locator.
    /// </summary>
    public Uri? Location { get; set; }

    /// <inheritdoc />
    public object Clone()
    {
        return new ExternalAsset
        {
            Id = Id,
            Name = Name,
            AdditionalProperties = new Dictionary<string, object>(AdditionalProperties),
            Description = Description,
            ContentType = ContentType,
            Location = Location is null
                ? null
                : new(Location.OriginalString)
        };
    }

    /// <summary>
    /// Write a local file to the <see cref="IStorageStrategy"/> and update the <see cref="Location"/>.
    /// </summary>
    public async Task<IResult> TryWriteLocalFileToStorage(IStorageStrategy storageStrategy)
    {
        if (Location is null)
            return new Failure($"{nameof(Location)} is not set.");
        if (!Location.IsFile)
            return new Failure($"{nameof(Location)} is not a local file.");
        Stream stream = File.OpenRead(Location.AbsolutePath);
        string fileName = Location.Segments.Last();
        IResult<Uri> result = await storageStrategy.WriteAsync(fileName, stream);
        if (result is not Success<Uri> success)
            return new Failure("Failed to write to storage.", result.Errors)
            {
                Warnings = result.Warnings
            };
        Location = success.Value;
        return new Success();
    }
}
