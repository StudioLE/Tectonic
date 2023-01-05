using Lineweights.Drawings.Samples;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Drawings.Tests.Samples;

internal sealed class SheetSampleTest
{
    public Model Model { get; } = new();

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
        Model.AddElements(outputs.Model.Elements.Values);

        // Assert
        // IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        // await Verify.ElementsByBounds(components);
    }

    [TearDown]
    public async Task TearDown()
    {
        await new Visualize().Execute(Model);
    }
}
