using Elements.Serialization.JSON;
using Newtonsoft.Json;

namespace Lineweights.Core.Serialisation;

/// <summary>
/// Serialise an interface or abstract to a concrete type.
/// </summary>
public class OverrideInheritanceConverter : JsonConverter<Element>
{
    private readonly JsonSerializerSettings _settings = new()
    {
        ContractResolver = new IgnoreConverterResolver(typeof(JsonInheritanceConverter))
    };

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, Element value, JsonSerializer serializer)
    {
        string json = JsonConvert.SerializeObject(value, _settings);
        writer.WriteValue(json);
    }

    /// <inheritdoc />
    public override Element ReadJson(
        JsonReader reader,
        Type objectType,
        Element? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.Value is not string json)
            throw new($"Failed to de-serialise {objectType}. Reader value was not a string.");
        object? obj = JsonConvert.DeserializeObject(json, objectType, _settings);
        if (obj is not Element element)
            throw new($"Failed to de-serialise {objectType}. De-serialised was not an Element.");
        return element;
    }
}
