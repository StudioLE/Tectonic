using Lineweights.Core.Documents;
using Lineweights.IFC;
using Lineweights.PDF;
using Lineweights.SVG;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Lineweights.Workflows.Tests.Documents;

internal sealed class AssetBuilderTests
{
    private readonly GeometricElement[] _geometry = Scenes.GeometricElements();

    [Test]
    public async Task AssetBuilder_ConvertModelToGlb()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        IAssetBuilder builder = new AssetBuilder()
            .ConvertModelToGlb();
        Asset asset = await builder.Build(model);

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = asset.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task AssetBuilder_ConvertModelToIfc()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        IAssetBuilder builder = new AssetBuilder()
            .ConvertModelToIfc();
        Asset asset = await builder.Build(model);

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = asset.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task AssetBuilder_ConvertModelToJson()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        IAssetBuilder builder = new AssetBuilder()
            .ConvertModelToJson();
        Asset asset = await builder.Build(model);

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
        Asset jsonAsset = asset.Children.First();
        Assert.That(jsonAsset.Info.Location, Is.Null, "Location");
        Assert.That(jsonAsset.Content, Is.Not.Empty, "Content");
    }

    [Test]
    public async Task AssetBuilder_ExtractViewsAndConvertToSvg()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        model.AddElements(SampleHelpers.CreateViews(_geometry));

        // Act
        IAssetBuilder builder = new AssetBuilder()
            .ExtractViewsAndConvertToSvg();
        Asset asset = await builder.Build(model);

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(3), "Children count");
        Uri? uri = asset.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task AssetBuilder_ExtractViewsAndConvertToPdf()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        model.AddElements(SampleHelpers.CreateViews(_geometry));

        // Act
        IAssetBuilder builder = new AssetBuilder()
            .ExtractViewsAndConvertToPdf();
        Asset asset = await builder.Build(model);

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(3), "Children count");
        Uri? uri = asset.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task AssetBuilder_by_DI()
    {
        // Arrange
        Model model = new();
        model.AddElements(SampleHelpers.CreateViews(_geometry));

        // Act
        IServiceProvider services = Services.GetInstance();
        IAssetBuilder builder =  services.GetRequiredService<IAssetBuilder>();
        Asset asset = await builder.Build(model);

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = asset.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task AssetBuilder_All()
    {
        // Arrange
        Model model = new();
        model.AddElements(SampleHelpers.CreateViews(_geometry));

        // Act
        IAssetBuilder builder = new AssetBuilder()
            .ConvertModelToGlb()
            .ConvertModelToIfc()
            .ExtractSheetsAndConvertToPdf()
            .ExtractSheetsAndConvertToSvg()
            .ExtractViewsAndConvertToPdf()
            .ExtractViewsAndConvertToSvg()
            // .ExtractDocumentInformation()
            .ConvertModelToJson();
        Asset asset = await builder.Build(model);

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(9), "Children count");
        Uri? uri = asset.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }
}
