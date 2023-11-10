using System.Diagnostics.CodeAnalysis;
using Cascade.Assets.Configuration;
using Elements;
using Geometrician.Core.Assets;
using Geometrician.Diagnostics.Samples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using StudioLE.Core.Results;

namespace Cascade.Assets.Tests.Documents;

internal sealed class AssetFactoryResolverTests
{
    private readonly IHost _host;
    private readonly Model _model;

    public AssetFactoryResolverTests()
    {
        _host = Host
            .CreateDefaultBuilder()
            .AddAssetFactoryServices()
            .Build();
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
    public void AssetFactoryResolver_GetFactoriesForSourceType(Type type, int expected)
    {
        // Arrange
        AssetFactoryResolver resolver = _host.Services.GetRequiredService<AssetFactoryResolver>();

        // Act
        IAssetFactory<IAsset>[] factories = resolver
            .ResolveForSourceType(type)
            .ToArray();

        // Assert
        Assert.That(factories.Length, Is.EqualTo(expected), "Count");
    }

    [Test]
    public async Task AssetFactoryResolver_GetFactoriesForObjectProperties()
    {
        // Arrange
        ExampleClass example = new(_model);
        AssetFactoryResolver resolver = _host.Services.GetRequiredService<AssetFactoryResolver>();

        // Act
        IAssetFactory<IAsset>[] factories = resolver
            .ResolveForObjectProperties(example)
            .ToArray();
        foreach (IAssetFactory<IAsset> factory in factories)
            if (factory.Result is not Success)
                await factory.Execute();

        // Assert
        Assert.That(factories.Length, Is.EqualTo(8), "Count");
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
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
