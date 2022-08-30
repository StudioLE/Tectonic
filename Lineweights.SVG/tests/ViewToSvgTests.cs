using System.IO;
using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.SVG.From.Elements;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.SVG.Tests;

[VisualizeInServerAppAfterTest]
internal sealed class ViewToSvgTests
{
    private readonly Model _model = new();
    private readonly ViewToSvg _converter = new();

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void ViewToSvg_Wireframe_Brickwork(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.Brickwork());
        View view = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(view);
        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        string svgString = svgDocument.ToString();

        // Preview
        Preview(svgDocument);

        // Assert
        Verify.String(svgString, ".svg");
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void ViewToSvg_Wireframe_GeometricElements(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.GeometricElements());
        View view = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(view);
        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        string svgString = svgDocument.ToString();

        // Preview
        Preview(svgDocument);

        // Assert
        Verify.String(svgString, ".svg");
    }
    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void ViewToSvg_Flat_Brickwork(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.Brickwork())
            .RenderStrategy(new FlatRender());
        View view = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(view);
        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        string svgString = svgDocument.ToString();

        // Preview
        Preview(svgDocument);

        // Assert
        Verify.String(svgString, ".svg");
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void ViewToSvg_Flat_GeometricElements(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.GeometricElements())
            .RenderStrategy(new FlatRender());
        View view = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(view);
        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        string svgString = svgDocument.ToString();

        // Preview
        Preview(svgDocument);

        // Assert
        Verify.String(svgString, ".svg");
    }

    private void Preview(SvgDocument svgDocument)
    {
        FileInfo file = TestHelpers.FileByTestContext("svg");
        svgDocument.Save(file.FullName);
        _model.AddElement(new DocumentInformation { Location = new(file.FullName) });
    }
}
