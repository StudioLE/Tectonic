using Elements.Serialization.JSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lineweights.Core.Serialization;

/// <summary>
/// Disable the behaviour of <see cref="JsonInheritanceConverter"/>.
/// </summary>
public class OverrideInheritanceConverter : JsonConverter<Element>
{
    private readonly JsonSerializer _serializer;

    public OverrideInheritanceConverter()
    {
        JsonSerializerSettings settings = new()
        {
            ContractResolver = new IgnoreConverterResolver(typeof(JsonInheritanceConverter))
        };
        _serializer = JsonSerializer.Create(settings);
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, Element value, JsonSerializer _)
    {
        JToken jToken = JToken.FromObject(value, _serializer);
        jToken.WriteTo(writer);
    }

    /// <inheritdoc/>
    public override Element ReadJson(
        JsonReader reader,
        Type objectType,
        Element? existingValue,
        bool hasExistingValue,
        JsonSerializer _)
    {
        object? obj = _serializer.Deserialize(reader, objectType);
        if (obj is not Element element)
            throw new($"Failed to de-serialise {objectType}. De-serialised was not an Element.");
        return element;
    }
}
