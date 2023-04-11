using System.IO;
using Newtonsoft.Json;

namespace Geometrician.Core.Serialization;

/// <summary>
/// Convert a <see cref="FileInfo"/> to and from JSON.
/// </summary>
public class FileInfoConverter : JsonConverter
{
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not FileInfo file)
            throw new($"Failed to serialise {nameof(FileInfo)}.");
        writer.WriteValue(file.FullName);
    }

    /// <inheritdoc/>
    public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
    {
        if (reader.Value is string path)
            return new FileInfo(path);
        throw new($"Failed to de-serialise {nameof(FileInfo)}.");
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(FileInfo);
    }
}
