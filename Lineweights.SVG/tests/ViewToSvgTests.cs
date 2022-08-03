using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.SVG.From.Elements;

namespace Lineweights.SVG.Tests;

internal sealed class ViewToSvgTests
{
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
        var converter = new ViewToSvg();
        SvgElement svgElement = converter.Convert(view);
        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        string svgString = svgDocument.ToString();

        // Preview
        //SampleHelpers.OpenInDebug(svgDocument);

        // Assert
        Verify.String(svgString);
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
        var converter = new ViewToSvg();
        SvgElement svgElement = converter.Convert(view);
        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        string svgString = svgDocument.ToString();

        // Preview
        //SampleHelpers.OpenInDebug(svgDocument);

        // Assert
        Verify.String(svgString);
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
        var converter = new ViewToSvg();
        SvgElement svgElement = converter.Convert(view);
        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        string svgString = svgDocument.ToString();

        // Preview
        //SampleHelpers.OpenInDebug(svgDocument);

        // Assert
        Verify.String(svgString);
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
        var converter = new ViewToSvg();
        SvgElement svgElement = converter.Convert(view);
        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        string svgString = svgDocument.ToString();

        // Preview
        //SampleHelpers.OpenInDebug(svgDocument);

        // Assert
        Verify.String(svgString);
    }
}
