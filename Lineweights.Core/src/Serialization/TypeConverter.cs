using Newtonsoft.Json;
using StudioLE.Core.System;

namespace Lineweights.Core.Serialization;

/// <summary>
/// Convert a <see cref="Type"/> to and from JSON.
/// The <see cref="Type"/> is serialised by its <see cref="Type.FullName"/>
/// and the name of its <see cref="Type.Assembly"/>.
/// </summary>
internal class TypeConverter : JsonConverter
{
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not Type type)
            throw new("Failed to serialise FileInfo.");

        string[] typeQualifiers = type
                                      .AssemblyQualifiedName
                                      ?.Split(',')
                                      .Take(2)
                                      .ToArray()
                                  ?? new[] { type.FullName };

        writer.WriteValue(typeQualifiers.Join(","));
    }

    /// <inheritdoc/>
    public override object? ReadJson(JsonReader reader, Type _, object value, JsonSerializer serializer)
    {
        if (reader.Value is string path)
            return Type.GetType(path);
        throw new($"Failed to de-serialise {nameof(Type)}.");
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return typeof(Type).IsAssignableFrom(objectType);
    }
}
