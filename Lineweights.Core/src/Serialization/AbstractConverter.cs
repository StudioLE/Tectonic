using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lineweights.Core.Serialization;

/// <summary>
/// Serialise an interface or abstract to a concrete type.
/// </summary>
public class AbstractConverter : JsonConverter
{
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Proxy proxy = new(value);
        serializer.Serialize(writer, proxy);
    }

    /// <inheritdoc/>
    public override object ReadJson(JsonReader jsonReader, Type type, object value, JsonSerializer serializer)
    {
        Proxy? proxy = serializer.Deserialize<Proxy>(jsonReader);
        if (proxy is null
            || proxy.Type is null
            || proxy.Properties is null)
            throw new($"JSON de-Serialization failed. JSON was not a {nameof(Proxy)}.");
        if (!type.IsAssignableFrom(proxy.Type))
            throw new($"JSON de-Serialization failed. {proxy.Type} is not assignable from {type}");
        object? result = proxy.Properties.ToObject(proxy.Type);
        if (result is null)
            throw new($"JSON de-Serialization failed. JSON was not a {type}.");
        return result;
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    private class Proxy
    {
        [JsonConverter(typeof(TypeConverter))]
        public Type? Type { get; }

        public JObject? Properties { get; }

        public Proxy()
        {
        }

        public Proxy(object obj)
        {
            Type = obj.GetType();
            Properties = JObject.FromObject(obj);
        }
    }
}
