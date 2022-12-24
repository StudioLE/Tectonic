using System.ComponentModel.DataAnnotations;
using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Samples;

public static class AssetTypes
{
    public sealed class Inputs
    {
        [Required]
        public Scenes.Name Scene { get; set; } = Scenes.Name.GeometricElements;

        [Required]
        public bool IncludeViewsInModel { get; set; } = true;

        [Required]
        public bool IncludeCsvFileAsAsset { get; set; } = true;

        [Required]
        public bool IncludeIfcFileAsAsset { get; set; } = true;

        [Required]
        public bool IncludeJsonAsContentAsset { get; set; } = true;

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
        Outputs outputs = new()
        {
            Model = Scenes.FromJson(inputs.Scene)
        };

        if (inputs.IncludeViewsInModel)
            outputs.Model.AddElements(SampleHelpers.CreateViews(outputs.Model));

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
