namespace Geometrician.Core.Configuration;

/// <summary>
/// A registry of viewer component types to be resolved by content type.
/// </summary>
/// <remarks>
/// The <see cref="ViewerComponentResolver"/> should be constructed via a <see cref="ViewerComponentResolverBuilder"/> and added to
/// to the dependency injection services.
/// </remarks>
public class ViewerComponentResolver
{
    private readonly IDictionary<string, Type> _contentTypes;

    internal ViewerComponentResolver(IDictionary<string, Type> contentTypes)
    {
        _contentTypes = contentTypes;
    }

    /// <summary>
    /// Resolve the component type for <paramref name="contentType"/>.
    /// </summary>
    /// <param name="contentType">The content type.</param>
    /// <returns>The resolved <see cref="Type"/> if it exists, otherwise <see langword="null"/>.</returns>
    public Type? Resolve(string contentType)
    {
        return _contentTypes.TryGetValue(contentType, out Type type)
            ? type
            : null;
    }
}
