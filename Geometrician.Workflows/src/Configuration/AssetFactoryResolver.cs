using System.Reflection;
using Geometrician.Workflows.Visualization;
using Geometrician.Core.Assets;
using Microsoft.Extensions.DependencyInjection;
using StudioLE.Core.Results;

namespace Geometrician.Workflows.Configuration;

/// <summary>
/// A registry of <see cref="IAssetFactory{TResult}"/> assigned to specific types.
/// </summary>
/// <remarks>
/// <para>
/// The registry is populated from <see cref="VisualizationConfiguration"/> by
/// <see href="https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection">dependency injection</see>.
/// </para>
/// <para>
/// The
/// </para>
/// The <see cref="ResolveForObjectProperties"/>
/// <see href="https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection">dependency injection</see>
/// </remarks>
public class AssetFactoryResolver
{
    private readonly IServiceProvider _services;
    private readonly IReadOnlyDictionary<Type, Type[]> _factories;
    private readonly IReadOnlyDictionary<Type, int> _order;

    internal AssetFactoryResolver(IServiceProvider services, IReadOnlyDictionary<Type, Type[]> factories, IReadOnlyDictionary<Type, int> order)
    {
        _services = services;
        _factories = factories;
        _order = order;
    }

    public IReadOnlyCollection<IAssetFactory<IAsset>> ResolveForObjectProperties(object obj)
    {
        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();
        return properties
            .SelectMany(property => ResolveForProperty(property, obj))
            .OrderBy(x => _order[x.GetType()])
            .ToArray();
    }

    private IEnumerable<IAssetFactory<IAsset>> ResolveForProperty(PropertyInfo property, object obj)
    {
        IEnumerable<IAssetFactory<IAsset>> factories = ResolveForSourceType(property.PropertyType)
            .Select(factory => SetupFactory(factory, property, obj))
            .OfType<IAssetFactory<IAsset>>();
        if (property.PropertyType != typeof(Model))
            return factories;
        Model model = property.GetValue(obj) as Model ?? throw new("Expected a Model.");
        IEnumerable<IAssetFactory<IAsset>> elementFactories = model
            .Elements
            .Values
            .SelectMany(element => ResolveForSourceType(element.GetType())
                .Select(factory => SetupFactory(factory, element)));
        return factories.Concat(elementFactories);
    }

    internal IEnumerable<IAssetFactory<IAsset>> ResolveForSourceType(Type type)
    {
        // TODO: This only gets exact matches so doesn't support polymorphism
        if (!_factories.ContainsKey(type))
            return Array.Empty<IAssetFactory<IAsset>>();
        Type[] factoryTypes = _factories[type];
        return factoryTypes
            .Select(_services.GetRequiredService)
            .OfType<IAssetFactory<IAsset>>();
    }

    private static IAssetFactory<IAsset>? SetupFactory(IAssetFactory<IAsset> factory, PropertyInfo property, object obj)
    {
        object sourceValue = property.GetValue(obj);
        if (sourceValue is null)
            return null;
        IResult<object> result = factory.TryInvokeMethod("Setup", new[] { sourceValue });
        return factory;
    }

    private static IAssetFactory<IAsset> SetupFactory(IAssetFactory<IAsset> factory, object sourceValue)
    {
        IResult<object> result = factory.TryInvokeMethod("Setup", new[] { sourceValue });
        return factory;
    }
}
