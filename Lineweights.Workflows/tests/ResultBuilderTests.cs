using Lineweights.Workflows.Results;

namespace Lineweights.Workflows.Tests;

[Explicit("Requires Azurite")]
[Category("Azurite")]
internal sealed class ResultBuilderTests
{
    private readonly GeometricElement[] _geometry = Scenes.GeometricElements();

    [Test]
    public void ResultBuilder_AddModelConvertedToGlb()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        ResultBuilder builder = new ResultBuilder()
            .AddModelConvertedToGlb(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Metadata.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
        Assert.That(uri?.IsFile, Is.False, "Uri IsFile");
    }

    [Test]
    public void ResultBuilder_AddModelConvertedToIfc()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        ResultBuilder builder = new ResultBuilder()
            .AddModelConvertedToIfc(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Metadata.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
        Assert.That(uri?.IsFile, Is.False, "Uri IsFile");
    }

    [Test]
    public void ResultBuilder_AddModelConvertedToJson()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);

        // Act
        ResultBuilder builder = new ResultBuilder()
            .AddModelConvertedToJson(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Metadata.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
        Assert.That(uri?.IsFile, Is.False, "Uri IsFile");
    }

    [Test]
    public void ResultBuilder_AddCanvasConvertedToSvg()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        model.AddElements(WorkflowSamples.Views(_geometry));

        // Act
        ResultBuilder builder = new ResultBuilder()
            .AddCanvasesConvertedToSvg(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(3), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Metadata.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
        Assert.That(uri?.IsFile, Is.False, "Uri IsFile");
    }

    [Test]
    public void ResultBuilder_AddCanvasConvertedToPdf()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        model.AddElements(WorkflowSamples.Views(_geometry));

        // Act
        ResultBuilder builder = new ResultBuilder()
            .AddCanvasesConvertedToPdf(model);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(3), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Metadata.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
        Assert.That(uri?.IsFile, Is.False, "Uri IsFile");
    }

    [Test]
    public void ResultBuilder_AddDocumentInformation()
    {
        // Arrange
        Model model = new();
        model.AddElements(_geometry);
        DocumentInformation doc = WorkflowSamples.CsvDocumentInformation(_geometry);
        model.AddElements(doc);

        // Act
        ResultBuilder builder = new ResultBuilder()
            .AddDocumentInformation(doc);
        Result result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(1), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Metadata.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
        Assert.That(uri?.IsFile, Is.False, "Uri IsFile");
    }

    [Test]
    public void ResultBuilder_Default()
    {
        // Arrange
        Model model = new();
        model.AddElements(WorkflowSamples.All());

        // Act
        Result result = ResultBuilder.Default(model, new());

        // Assert
        Assert.That(result, Is.Not.Null, "Not null");
        Assert.That(result.Children.Count, Is.EqualTo(10), "Children count");
        Uri? uri = result.Children.FirstOrDefault()?.Metadata.Location;
        Assert.That(uri, Is.Not.Null, "Uri is not null");
        Assert.That(uri?.IsFile, Is.False, "Uri IsFile");
    }
}
