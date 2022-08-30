using Lineweights.Workflows.Assets;

namespace Lineweights.Workflows.Tests.Assets;

internal sealed class AssetBuilderTests
{
    private readonly GeometricElement[] _geometry = Scenes.GeometricElements();
    private readonly IStorageStrategy _storageStrategy = new FileStorageStrategy();

    [Test]
    public async Task AssetBuilder_ConvertModelToGlb()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        AssetBuilder builder = new AssetBuilder(_storageStrategy)
            .ConvertModelToGlb(model);
        Asset asset = await builder.Build();

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
        AssetBuilder builder = new AssetBuilder(_storageStrategy)
            .ConvertModelToIfc(model);
        Asset asset = await builder.Build();

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
        AssetBuilder builder = new AssetBuilder(_storageStrategy)
            .ConvertModelToJson(model);
        Asset asset = await builder.Build();

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
        AssetBuilder builder = new AssetBuilder(_storageStrategy)
            .ExtractViewsAndConvertToSvg(model);
        Asset asset = await builder.Build();

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
        AssetBuilder builder = new AssetBuilder(_storageStrategy)
            .ExtractViewsAndConvertToPdf(model);
        Asset asset = await builder.Build();

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(3), "Children count");
        Uri? uri = asset.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task AssetBuilder_Default()
    {
        // Arrange
        Model model = new();
        model.AddElements(SampleHelpers.CreateViews(_geometry));

        // Act
        AssetBuilder builder = AssetBuilder.Default(_storageStrategy, model);
        Asset asset = await builder.Build();

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(4), "Children count");
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
        Asset asset = await new AssetBuilder(_storageStrategy)
            .ConvertModelToGlb(model)
            .ConvertModelToIfc(model)
            .ExtractSheetsAndConvertToPdf(model)
            .ExtractSheetsAndConvertToSvg(model)
            .ExtractViewsAndConvertToPdf(model)
            .ExtractViewsAndConvertToSvg(model)
            // .ExtractDocumentInformation(model)
            .ConvertModelToJson(model)
            .Build();

        // Assert
        Assert.That(asset, Is.Not.Null, "Asset");
        Assert.That(asset.Children.Count, Is.EqualTo(9), "Children count");
        Uri? uri = asset.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }
}
