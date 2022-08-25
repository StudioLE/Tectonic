namespace Lineweights.Workflows.Samples;

public sealed class DashboardSamples
{
    public sealed class Outputs
    {
        public Model Model { get; set; } = new();
    }

    public static Outputs Dashboard_Sample_Geometry()
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Outputs outputs = new();
        outputs.Model.AddElements(geometry);
        outputs.Model.AddElements(ResultSamples.Views(geometry));
        return outputs;
    }

    public static Outputs Dashboard_Sample_Views()
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Outputs outputs = new();
        outputs.Model.AddElements(geometry);
        outputs.Model.AddElements(ResultSamples.Views(geometry));
        return outputs;
    }

    public static Outputs Dashboard_Sample_Csv()
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Outputs outputs = new();
        outputs.Model.AddElements(geometry);
        DocumentInformation doc = ResultSamples.CsvDocumentInformation(outputs.Model);
        outputs.Model.AddElement(doc);
        return outputs;
    }

    public static Outputs Dashboard_Sample_Ifc()
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Outputs outputs = new();
        outputs.Model.AddElements(geometry);
        DocumentInformation doc = ResultSamples.IfcDocumentInformation(outputs.Model);
        outputs.Model.AddElement(doc);
        return outputs;
    }

    public static Outputs Dashboard_Sample_Json()
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Outputs outputs = new();
        outputs.Model.AddElements(geometry);
        DocumentInformation doc = ResultSamples.JsonDocumentInformation(outputs.Model);
        outputs.Model.AddElement(doc);
        return outputs;
    }

    public static Outputs Dashboard_Sample_All()
    {
        Outputs outputs = new();
        outputs.Model.AddElements(ResultSamples.All());
        return outputs;
    }

    public static Outputs Dashboard_Sample_Throw_NotImplementedException()
    {
        throw new("This exception is thrown intentionally to test exception handling.");
    }
}
