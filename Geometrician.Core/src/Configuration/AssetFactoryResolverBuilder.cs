using Lineweights.Core.Assets;

namespace Geometrician.Core.Configuration;

/// <summary>
/// Build an <see cref="AssetFactoryResolver"/>.
/// </summary>
public class AssetFactoryResolverBuilder : IBuilder<AssetFactoryResolver>
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<Type,Type[]> _factories = new();
    private readonly Dictionary<Type, int> _order = new();

    /// <inheritdoc cref="AssetFactoryResolverBuilder"/>
    public AssetFactoryResolverBuilder(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Register <typeparamref name="TAssetFactory"/> as an <see cref="IAssetFactory{TResult}"/> for
    /// elements of type <see cref="TSource"/>.
    /// </summary>
    /// <param name="order">An optional value used to order the <see cref="IAssetFactory{TResult}"/> when resolved.</param>
    /// <returns>The <see cref="AssetFactoryResolverBuilder"/>.</returns>
    public AssetFactoryResolverBuilder Register<TSource, TAssetFactory>(int? order = null) where TAssetFactory : IAssetFactory<TSource, IAsset>
    {
        order ??= _order.Count + 100;
        Type sourceType = typeof(TSource);
        Type converterType = typeof(TAssetFactory);
        Type[] converters = _factories.ContainsKey(sourceType)
            ? _factories[sourceType]
            : Array.Empty<Type>();
        converters = converters.Append(converterType).ToArray();
        _factories[sourceType] = converters;
        _order[converterType] = (int)order;
        return this;
    }

    /// <inheritdoc />
    public AssetFactoryResolver Build()
    {
        return new(_services, _factories, _order);
    }
}
