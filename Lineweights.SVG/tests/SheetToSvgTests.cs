using System.IO;
using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.Flex;
using Lineweights.SVG.From.Elements;
using Lineweights.Workflows.Results;

namespace Lineweights.SVG.Tests;

[SendToDashboardAfterTest]
internal sealed class SheetToSvgTests : ResultModel
{
    private readonly SheetToSvg _converter = new();
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();

    [Test]
    public void SheetToSvg_Wireframe_Brickwork()
    {
        // Arrange
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Left,
            ViewDirection.Right,
            ViewDirection.Front,
            ViewDirection.Back
        };

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .Scale(1d / 10)
            .ElementsInView(_brickwork);

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        SheetBuilder builder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(Justification.SpaceEvenly);
                viewArrangement.CrossJustification(Justification.SpaceEvenly);
            });
        Sheet sheet = builder.Build();

        // Act
        SvgDocument svgDocument = _converter.Convert(sheet);
        string svgString = svgDocument.ToString();

        // Preview
        Preview(svgDocument);

        // Assert
        Verify.String(svgString, ".svg");
    }

    [Test]
    public void SheetToSvg_Wireframe_GeometricElements()
    {
        // Arrange
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Back,
            ViewDirection.Front,
            ViewDirection.Left,
            ViewDirection.Right
        };

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .Scale(1d / 5)
            .ElementsInView(_geometry);

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        SheetBuilder builder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(Justification.SpaceEvenly);
                viewArrangement.CrossJustification(Justification.SpaceEvenly);
            });
        Sheet sheet = builder.Build();

        // Act
        SvgDocument svgDocument = _converter.Convert(sheet);
        string svgString = svgDocument.ToString();

        // Preview
        Preview(svgDocument);

        // Assert
        Verify.String(svgString, ".svg");
    }
    [Test]
    public void SheetToSvg_Flat_Brickwork()
    {
        // Arrange
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Left,
            ViewDirection.Right,
            ViewDirection.Front,
            ViewDirection.Back
        };

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .Scale(1d / 10)
            .ElementsInView(_brickwork)
            .RenderStrategy(new FlatRender());

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        SheetBuilder builder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(Justification.SpaceEvenly);
                viewArrangement.CrossJustification(Justification.SpaceEvenly);
            });
        Sheet sheet = builder.Build();

        // Act
        SvgDocument svgDocument = _converter.Convert(sheet);
        string svgString = svgDocument.ToString();

        // Preview
        Preview(svgDocument);

        // Assert
        Verify.String(svgString, ".svg");
    }

    [Test]
    public void SheetToSvg_Flat_GeometricElements()
    {
        // Arrange
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Back,
            ViewDirection.Front,
            ViewDirection.Left,
            ViewDirection.Right
        };

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .Scale(1d / 5)
            .ElementsInView(_geometry)
            .RenderStrategy(new FlatRender());

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        SheetBuilder builder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(Justification.SpaceEvenly);
                viewArrangement.CrossJustification(Justification.SpaceEvenly);
            });
        Sheet sheet = builder.Build();

        // Act
        SvgDocument svgDocument = _converter.Convert(sheet);
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
        Model.AddElement(new DocumentInformation { Location = new(file.FullName) });
    }
}
