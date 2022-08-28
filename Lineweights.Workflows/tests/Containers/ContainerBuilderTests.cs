using Lineweights.Workflows.Containers;

namespace Lineweights.Workflows.Tests.Containers;

internal sealed class ContainerBuilderTests
{
    private readonly GeometricElement[] _geometry = Scenes.GeometricElements();
    private readonly IStorageStrategy _storageStrategy = new FileStorageStrategy();

    [Test]
    public async Task ContainerBuilder_ConvertModelToGlb()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        ContainerBuilder builder = new ContainerBuilder(_storageStrategy)
            .ConvertModelToGlb(model);
        Container container = await builder.Build();

        // Assert
        Assert.That(container, Is.Not.Null, "Container");
        Assert.That(container.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = container.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task ContainerBuilder_ConvertModelToIfc()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        ContainerBuilder builder = new ContainerBuilder(_storageStrategy)
            .ConvertModelToIfc(model);
        Container container = await builder.Build();

        // Assert
        Assert.That(container, Is.Not.Null, "Container");
        Assert.That(container.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = container.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task ContainerBuilder_ConvertModelToJson()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        ContainerBuilder builder = new ContainerBuilder(_storageStrategy)
            .ConvertModelToJson(model);
        Container container = await builder.Build();

        // Assert
        Assert.That(container, Is.Not.Null, "Container");
        Assert.That(container.Children.Count, Is.EqualTo(1), "Children count");
        Container jsonContainer = container.Children.First();
        Assert.That(jsonContainer.Info.Location, Is.Null, "Location");
        Assert.That(jsonContainer.Content, Is.Not.Empty, "Content");
    }

    [Test]
    public async Task ContainerBuilder_ExtractViewsAndConvertToSvg()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        model.AddElements(SampleHelpers.CreateViews(_geometry));

        // Act
        ContainerBuilder builder = new ContainerBuilder(_storageStrategy)
            .ExtractViewsAndConvertToSvg(model);
        Container container = await builder.Build();

        // Assert
        Assert.That(container, Is.Not.Null, "Container");
        Assert.That(container.Children.Count, Is.EqualTo(3), "Children count");
        Uri? uri = container.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task ContainerBuilder_ExtractViewsAndConvertToPdf()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        model.AddElements(SampleHelpers.CreateViews(_geometry));

        // Act
        ContainerBuilder builder = new ContainerBuilder(_storageStrategy)
            .ExtractViewsAndConvertToPdf(model);
        Container container = await builder.Build();

        // Assert
        Assert.That(container, Is.Not.Null, "Container");
        Assert.That(container.Children.Count, Is.EqualTo(3), "Children count");
        Uri? uri = container.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    // [Test]
    // public async Task ContainerBuilder_AddDocumentInformation()
    // {
    //     // Arrange
    //     Model model = new();
    //     model.AddElements(_geometry);
    //     DocumentInformation doc = ResultSamples.CsvDocumentInformation(model);
    //     model.AddElements(doc);
    //
    //     // Act
    //     ContainerBuilder builder = new ContainerBuilder(_storageStrategy)
    //         .AddDocumentInformation(doc, "text/csv");
    //     Container container = await builder.Build();
    //
    //     // Assert
    //     Assert.That(container, Is.Not.Null, "Container");
    //     Assert.That(container.Children.Count, Is.EqualTo(1), "Children count");
    //     Uri? uri = container.Children.FirstOrDefault()?.Info.Location;
    //     Assert.That(uri, Is.Not.Null, "Location");
    // }

    [Test]
    public async Task ContainerBuilder_Default()
    {
        // Arrange
        Model model = new();
        model.AddElements(SampleHelpers.CreateViews(_geometry));

        // Act
        ContainerBuilder builder = ContainerBuilder.Default(_storageStrategy, model);
        Container container = await builder.Build();

        // Assert
        Assert.That(container, Is.Not.Null, "Container");
        Assert.That(container.Children.Count, Is.EqualTo(4), "Children count");
        Uri? uri = container.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }

    [Test]
    public async Task ContainerBuilder_All()
    {
        // Arrange
        Model model = new();
        model.AddElements(SampleHelpers.CreateViews(_geometry));

        // Act
        Container container = await new ContainerBuilder(_storageStrategy)
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
        Assert.That(container, Is.Not.Null, "Container");
        Assert.That(container.Children.Count, Is.EqualTo(9), "Children count");
        Uri? uri = container.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Location");
    }
}
