using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lineweights.Core.Serialization;

/// <summary>
/// Ignore the specified converters.
/// </summary>
/// <inheritdoc cref="DefaultContractResolver"/>
/// <see href="https://stackoverflow.com/a/53402583/247218"/>
public class IgnoreConverterResolver : DefaultContractResolver
{
    private readonly HashSet<Type> _converters;

    /// <inheritdoc cref="IgnoreConverterResolver"/>
    public IgnoreConverterResolver(params Type[] converters)
    {
        _converters = new(converters);
    }

    /// <inheritdoc/>
    protected override JsonConverter? ResolveContractConverter(Type objectType)
    {
        JsonConverter? converter = base.ResolveContractConverter(objectType);
        if (converter is null)
            return null;
        return _converters.Contains(converter.GetType())
            ? null
            : converter;
    }
}
