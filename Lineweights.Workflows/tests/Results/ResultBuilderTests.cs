using Lineweights.Workflows.Results;

namespace Lineweights.Workflows.Tests.Results;

internal sealed class ResultBuilderTests
{
    private readonly GeometricElement[] _geometry = Scenes.GeometricElements();
    private readonly IStorageStrategy _storageStrategy = new FileStorageStrategy();

    [Test]
    public void ResultBuilder_ConvertModelToGlb()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        ResultBuilder builder = new ResultBuilder(_storageStrategy)
            .ConvertModelToGlb(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
    }

    [Test]
    public void ResultBuilder_ConvertModelToIfc()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        ResultBuilder builder = new ResultBuilder(_storageStrategy)
            .ConvertModelToIfc(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
    }

    [Test]
    public void ResultBuilder_ConvertModelToJson()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        ResultBuilder builder = new ResultBuilder(_storageStrategy)
            .ConvertModelToJson(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
    }

    [Test]
    public void ResultBuilder_ExtractViewsAndConvertToSvg()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        var views = ResultSamples.Views(_geometry);
        model.AddElements(views);

        // Act
        ResultBuilder builder = new ResultBuilder(_storageStrategy)
            .ExtractViewsAndConvertToSvg(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(3), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
    }

    [Test]
    public void ResultBuilder_ExtractViewsAndConvertToPdf()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        model.AddElements(ResultSamples.Views(_geometry));

        // Act
        ResultBuilder builder = new ResultBuilder(_storageStrategy)
            .ExtractViewsAndConvertToPdf(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(3), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
    }

    [Test]
    public void ResultBuilder_AddDocumentInformation()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        DocumentInformation doc = ResultSamples.CsvDocumentInformation(model);
        model.AddElements(doc);

        // Act
        ResultBuilder builder = new ResultBuilder(_storageStrategy)
            .AddDocumentInformation(doc);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
    }

    [Test]
    public void ResultBuilder_Default()
    {
        // Arrange
        Model model = new();
        model.AddElements(ResultSamples.All());

        // Act
        Result result = ResultBuilder.Default(_storageStrategy, model);

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(7), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
    }

    [Test]
    public void ResultBuilder_All()
    {
        // Arrange
        Model model = new();
        model.AddElements(ResultSamples.All());

        // Act
        Result result = new ResultBuilder(_storageStrategy)
            .ConvertModelToGlb(model)
            .ConvertModelToIfc(model)
            .ExtractSheetsAndConvertToPdf(model)
            .ExtractSheetsAndConvertToSvg(model)
            .ExtractViewsAndConvertToPdf(model)
            .ExtractViewsAndConvertToSvg(model)
            .ExtractDocumentInformation(model)
            .ConvertModelToJson(model)
            .Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(12), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Info.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
    }
}
