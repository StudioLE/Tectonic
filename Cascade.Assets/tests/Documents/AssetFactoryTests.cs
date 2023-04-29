using System.IO;
using Geometrician.Core.Assets;
using Geometrician.Core.Storage;
using Geometrician.Diagnostics.Samples;
using Geometrician.Drawings;
using Geometrician.IFC;
using Geometrician.PDF;
using Geometrician.SVG;
using Cascade.Assets.Factories;
using Cascade.Assets.Storage;
using NUnit.Framework;
using StudioLE.Core.Results;

namespace Cascade.Assets.Tests.Documents;

internal sealed class AssetFactoryTests
{
    private readonly GeometricElement[] _geometry = Scenes.GeometricElements();
    private readonly IStorageStrategy _storageStrategy = new FileStorageStrategy();

    [Test]
    public void AssetFactory_Setup_ExternalAsset_Null()
    {
        // Arrange
        ExternalAsset source = new()
        {
            Location = null
        };
        AssetFactory factory = new(_storageStrategy);

        // Act
        factory.Setup(source);

        // Assert
        Assert.That(factory.Asset, Is.EqualTo(source), "Asset.");
        Assert.That(factory.Result, Is.TypeOf<Failure>(), "Execution.");
    }

    [Test]
    public void AssetFactory_Setup_ExternalAsset_Local()
    {
        // Arrange
        string path = Path.GetTempFileName();
        ExternalAsset source = new()
        {
            Location = new(path)
        };
        AssetFactory factory = new(_storageStrategy);

        // Act
        factory.Setup(source);

        // Assert
        Assert.That(factory.Asset, Is.EqualTo(source), "Asset.");
        Assert.That(factory.Result, Is.TypeOf<NotExecuted>(), "Execution.");
    }

    [Test]
    public void AssetFactory_Setup_ExternalAsset_Remote()
    {
        // Arrange
        const string path = "https://placehold.co/600x400";
        ExternalAsset source = new()
        {
            Location = new(path)
        };
        AssetFactory factory = new(_storageStrategy);

        // Act
        factory.Setup(source);

        // Assert
        Assert.That(factory.Asset, Is.EqualTo(source), "Asset.");
        Assert.That(factory.Result, Is.TypeOf<Success>(), "Execution.");
    }

    [Test]
    public void AssetFactory_Setup_InternalAsset()
    {
        // Arrange
        InternalAsset source = new()
        {
            Content = "Hello, world!"
        };
        AssetFactory factory = new(_storageStrategy);

        // Act
        factory.Setup(source);

        // Assert
        Assert.That(factory.Asset, Is.EqualTo(source), "Asset.");
        Assert.That(factory.Result, Is.TypeOf<Success>(), "Execution.");
    }

    [Test]
    public async Task CsvElementTypesAssetFactory_Execute()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        CsvElementTypesAssetFactory factory = new(_storageStrategy);

        // Act
        factory.Setup(model);
        await factory.Execute();

        // Assert
        Assert.That(factory.Asset, Is.Not.Null, "Asset.");
        Assert.That(factory.Asset, Is.TypeOf<ExternalAsset>(), "Asset type.");
        Assert.That(factory.Asset.Location, Is.Not.Null, "Asset location");
        Assert.That(File.Exists(factory.Asset.Location!.AbsolutePath), "Asset file exists");
        Assert.That(factory.Asset.Location!.Segments.Last(), Does.EndWith(".csv"), "Asset file extension");
    }

    [Test]
    public async Task GlbAssetFactory_Execute()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        GlbAssetFactory factory = new(_storageStrategy);

        // Act
        factory.Setup(model);
        await factory.Execute();

        // Assert
        Assert.That(factory.Asset, Is.Not.Null, "Asset.");
        Assert.That(factory.Asset, Is.TypeOf<ExternalAsset>(), "Asset type.");
        Assert.That(factory.Asset.Location, Is.Not.Null, "Asset location");
        Assert.That(File.Exists(factory.Asset.Location!.AbsolutePath), "Asset file exists");
        Assert.That(factory.Asset.Location!.Segments.Last(), Does.EndWith(".glb"), "Asset file extension");
    }

    [Test]
    public async Task IfcAssetFactory_Execute()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        IfcAssetFactory factory = new(_storageStrategy);

        // Act
        factory.Setup(model);
        await factory.Execute();

        // Assert
        Assert.That(factory.Asset, Is.Not.Null, "Asset.");
        Assert.That(factory.Asset, Is.TypeOf<ExternalAsset>(), "Asset type.");
        Assert.That(factory.Asset.Location, Is.Not.Null, "Asset location");
        Assert.That(File.Exists(factory.Asset.Location!.AbsolutePath), "Asset file exists");
        Assert.That(factory.Asset.Location!.Segments.Last(), Does.EndWith(".ifc"), "Asset file extension");
    }

    [Test]
    public async Task JsonAssetFactory_Execute()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        JsonAssetFactory factory = new();

        // Act
        factory.Setup(model);
        await factory.Execute();

        // Assert
        Assert.That(factory.Asset, Is.Not.Null, "Asset.");
        Assert.That(factory.Asset, Is.TypeOf<InternalAsset>(), "Asset type.");
        Assert.That(factory.Asset.Content, Is.Not.Empty, "Asset content");
    }

    [Test]
    public async Task PdfAssetFactory_Execute()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        IReadOnlyCollection<View> views = SampleHelpers.CreateViews(model, Scenes.Name.GeometricElements);
        PdfAssetFactory<View> factory = new(_storageStrategy);

        // Act
        factory.Setup(views.First());
        await factory.Execute();

        // Assert
        Assert.That(factory.Asset, Is.Not.Null, "Asset.");
        Assert.That(factory.Asset, Is.TypeOf<ExternalAsset>(), "Asset type.");
        Assert.That(factory.Asset.Location, Is.Not.Null, "Asset location");
        Assert.That(File.Exists(factory.Asset.Location!.AbsolutePath), "Asset file exists");
        Assert.That(factory.Asset.Location!.Segments.Last(), Does.EndWith(".pdf"), "Asset file extension");
    }

    [Test]
    public async Task SvgAssetFactory_Execute()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        IReadOnlyCollection<View> views = SampleHelpers.CreateViews(model, Scenes.Name.GeometricElements);
        SvgAssetFactory<View> factory = new(_storageStrategy);

        // Act
        factory.Setup(views.First());
        await factory.Execute();

        // Assert
        Assert.That(factory.Asset, Is.Not.Null, "Asset.");
        Assert.That(factory.Asset, Is.TypeOf<ExternalAsset>(), "Asset type.");
        Assert.That(factory.Asset.Location, Is.Not.Null, "Asset location");
        Assert.That(File.Exists(factory.Asset.Location!.AbsolutePath), "Asset file exists");
        Assert.That(factory.Asset.Location!.Segments.Last(), Does.EndWith(".svg"), "Asset file extension");
    }
}
