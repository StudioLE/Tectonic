using System.ComponentModel.DataAnnotations;
using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Samples;

public static class GeometricScene
{
    public sealed class Inputs
    {
        [Required]
        public bool IncludeViewsInModel { get; set; } = false;

        [Required]
        public bool IncludeCsvFileAsAsset { get; set; } = false;

        [Required]
        public bool IncludeIfcFileAsAsset { get; set; } = false;

        [Required]
        public bool IncludeJsonAsContentAsset { get; set; } = false;

        [Required]
        public bool ThrowAnException { get; set; } = false;
    }

    public sealed class Outputs
    {
        public Model Model { get; set; } = new();

        public List<Asset> Assets { get; } = new();
    }

    public static Outputs Execute(Inputs inputs)
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Outputs outputs = new();
        outputs.Model.AddElements(geometry);

        if (inputs.IncludeViewsInModel)
            outputs.Model.AddElements(SampleHelpers.CreateViews(geometry));

        if (inputs.IncludeCsvFileAsAsset)
            outputs.Assets.Add(SampleHelpers.CreateCsvFileAsAsset(outputs.Model));

        if (inputs.IncludeIfcFileAsAsset)
            outputs.Assets.Add(SampleHelpers.CreateIfcFileAsAsset(outputs.Model));

        if (inputs.IncludeJsonAsContentAsset)
            outputs.Assets.Add(SampleHelpers.CreateJsonAsContentAsset(outputs.Model));

        if (inputs.ThrowAnException)
            throw new("This exception is thrown intentionally to test exception handling.");

        return outputs;
    }
}
