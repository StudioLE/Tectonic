using System.Reflection;
using Geometrician.Core.Visualization;
using Lineweights.Core.Assets;
using Microsoft.Extensions.DependencyInjection;
using StudioLE.Core.Results;

namespace Geometrician.Core.Assets;

public class AssetFactoryProvider
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<Type, Type[]> _factories;
    private readonly Dictionary<Type, int> _order;

    public AssetFactoryProvider(IServiceProvider services, VisualizationConfiguration configuration)
    {
        _services = services;
        _factories = configuration.AssetFactories;
        _order = configuration.AssertFactoriesOrder;
    }

    public IReadOnlyCollection<IAssetFactory<IAsset>> GetFactoriesForObjectProperties(object obj)
    {
        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();
        return properties
            .SelectMany(property => GetFactoriesForProperty(property, obj))
            .OrderBy(x => _order[x.GetType()])
            .ToArray();
    }

    private IEnumerable<IAssetFactory<IAsset>> GetFactoriesForProperty(PropertyInfo property, object obj)
    {
        IEnumerable<IAssetFactory<IAsset>> factories = GetFactoriesForSourceType(property.PropertyType)
            .Select(factory => SetupFactory(factory, property, obj))
            .OfType<IAssetFactory<IAsset>>();
        if (property.PropertyType != typeof(Model))
            return factories;
        Model model = property.GetValue(obj) as Model ?? throw new("Expected a Model.");
        IEnumerable<IAssetFactory<IAsset>> elementFactories = model
            .Elements
            .Values
            .SelectMany(element => GetFactoriesForSourceType(element.GetType())
                .Select(factory => SetupFactory(factory, element)));
        return factories.Concat(elementFactories);
    }

    internal IEnumerable<IAssetFactory<IAsset>> GetFactoriesForSourceType(Type type)
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
        IResult<object> result = factory.TryInvokeMethod("Setup", new []{ sourceValue });
        return factory;
    }

    private static IAssetFactory<IAsset> SetupFactory(IAssetFactory<IAsset> factory, object sourceValue)
    {
        IResult<object> result = factory.TryInvokeMethod("Setup", new []{ sourceValue });
        return factory;
    }
}
