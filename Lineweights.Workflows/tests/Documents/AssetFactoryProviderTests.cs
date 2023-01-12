using Lineweights.Core.Documents;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StudioLE.Core.Results;

namespace Lineweights.Workflows.Tests.Documents;

internal sealed class AssetFactoryProviderTests
{
    private readonly IServiceProvider _services;
    private readonly Model _model;

    public AssetFactoryProviderTests()
    {
        _services = Services.GetInstance();
        _model = new();
        _model.AddElements(Scenes.GeometricElements());
        ExternalAsset externalAsset = new()
        {
            Location = new("https://placehold.co/600x400")
        };
        InternalAsset internalAsset = new()
        {
            Content = "Hello, world!"
        };
        _model.AddElement(externalAsset);
        _model.AddElement(internalAsset);
    }

    [TestCase(typeof(ExternalAsset), 1)]
    [TestCase(typeof(InternalAsset), 1)]
    [TestCase(typeof(Model), 3)]
    public void AssetFactoryProvider_GetFactoriesForSourceType(Type type, int expected)
    {
        // Arrange
        AssetFactoryProvider provider = _services.GetRequiredService<AssetFactoryProvider>();

        // Act
        IAssetFactory<IAsset>[] factories = provider
            .GetFactoriesForSourceType(type)
            .ToArray();

        // Assert
        Assert.That(factories.Length, Is.EqualTo(expected), "Count");
    }

    [Test]
    public async Task AssetFactoryProvider_GetFactoriesForObjectProperties()
    {
        // Arrange
        ExampleClass example = new(_model);
        AssetFactoryProvider provider = _services.GetRequiredService<AssetFactoryProvider>();

        // Act
        IAssetFactory<IAsset>[] factories = provider
            .GetFactoriesForObjectProperties(example)
            .ToArray();
        foreach (IAssetFactory<IAsset> factory in factories)
            if(factory.Result is not Success)
                await factory.Execute();

        // Assert
        Assert.That(factories.Length, Is.EqualTo(8), "Count");
    }

    private class ExampleClass
    {
        public Model Model { get; }

        public ExternalAsset ExternalAsset { get; }

        public InternalAsset InternalAsset { get; }

        public ExternalAsset? NullableAssetSet { get; }

        public ExternalAsset? NullableAssetNull { get; }

        public ExampleClass(Model model)
        {
            Model = model;
            ExternalAsset = new()
            {
                Location = new("https://placehold.co/600x400")
            };
            InternalAsset = new()
            {
                Content = "Hello, world!"
            };
            NullableAssetNull = null;
            NullableAssetSet = new()
            {
                Location = new("https://placehold.co/600x400")
            };
        }
    }
}
