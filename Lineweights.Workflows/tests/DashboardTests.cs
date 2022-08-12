using Lineweights.Workflows.Results;

namespace Lineweights.Workflows.Tests;

[SendToDashboardAfterTest]
internal sealed class DashboardTests : ResultModel
{
    // TODO: Once IWorkflowRunner exists this can be removed

    [Test]
    public void Dashboard_Sample_Geometry()
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Model.AddElements(geometry);
        Model.AddElements(WorkflowSamples.Views(geometry));
    }

    [Test]
    public void Dashboard_Sample_Views()
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Model.AddElements(geometry);
        Model.AddElements(WorkflowSamples.Views(geometry));
    }

    [Test]
    public void Dashboard_Sample_Csv()
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Model.AddElements(geometry);
        Model.AddElements(WorkflowSamples.CsvDocumentInformation(geometry));
    }

    [Test]
    public void Dashboard_Sample_All()
    {
        Model.AddElements(WorkflowSamples.All());
    }
}
