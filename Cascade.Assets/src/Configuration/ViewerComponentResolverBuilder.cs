using Geometrician.Core.Abstractions;

namespace Cascade.Assets.Configuration;

/// <summary>
/// Build an <see cref="ViewerComponentResolver"/>.
/// </summary>
public class ViewerComponentResolverBuilder : IBuilder<ViewerComponentResolver>
{
    private readonly Dictionary<string, Type> _contentTypes = new();

    /// <summary>
    /// Add <paramref name="component"/> to the registry to be resolved for <paramref name="contentType"/>.
    /// </summary>
    /// <param name="contentType">The content type to register a <see cref="Type"/> for.</param>
    /// <param name="component">The <see cref="Type"/> of the component to register.</param>
    /// <returns>The <see cref="ViewerComponentResolverBuilder"/>.</returns>
    public ViewerComponentResolverBuilder Register(string contentType, Type component)
    {
        _contentTypes[contentType] = component;
        return this;
    }

    /// <inheritdoc/>
    public ViewerComponentResolver Build()
    {
        return new(_contentTypes);
    }
}
