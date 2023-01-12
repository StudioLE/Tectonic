using Lineweights.Diagnostics.NUnit.Visualization;
using Lineweights.Drawings.Samples;

namespace Lineweights.Drawings.Tests.Samples;

internal sealed class SheetSampleTest
{
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public void SheetSample_Execute()
    {
        // Arrange
        SheetSample.ViewInputs viewInputs = new();
        SheetSample.SheetInputs sheetInputs = new();
        SheetSample.ArrangementInputs arrangementInputs = new();

        // Act
        SheetSample.Outputs outputs = SheetSample.Execute(viewInputs, sheetInputs, arrangementInputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        // IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        // await Verify.ElementsByBounds(components);
    }

    [TearDown]
    public void TearDown()
    {
        _visualize.Queue(_model);
        _model = new();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _visualize.Execute();
    }
}
