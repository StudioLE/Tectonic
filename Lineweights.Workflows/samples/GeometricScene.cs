using System.ComponentModel.DataAnnotations;
using Lineweights.Workflows.Containers;

namespace Lineweights.Workflows.Samples;

public static class GeometricScene
{
    public sealed class Inputs
    {
        [Required]
        public bool IncludeViewsInModel { get; set; } = false;

        [Required]
        public bool IncludeCsvFileContainer { get; set; } = false;

        [Required]
        public bool IncludeIfcFileContainer { get; set; } = false;

        [Required]
        public bool IncludeJsonContentContainer { get; set; } = false;

        [Required]
        public bool ThrowAnException { get; set; } = false;
    }

    public sealed class Outputs
    {
        public Model Model { get; set; } = new();

        public List<Container> Containers { get; }= new();
    }

    public static Outputs Execute(Inputs inputs)
    {
        GeometricElement[] geometry = Scenes.GeometricElements();
        Outputs outputs = new();
        outputs.Model.AddElements(geometry);

        if (inputs.IncludeViewsInModel)
            outputs.Model.AddElements(SampleHelpers.CreateViews(geometry));

        if (inputs.IncludeCsvFileContainer)
            outputs.Containers.Add(SampleHelpers.CreateCsvFileContainer(outputs.Model));

        if (inputs.IncludeIfcFileContainer)
            outputs.Containers.Add(SampleHelpers.CreateIfcFileContainer(outputs.Model));

        if (inputs.IncludeJsonContentContainer)
            outputs.Containers.Add(SampleHelpers.CreateJsonContainer(outputs.Model));

        if (inputs.ThrowAnException)
            throw new("This exception is thrown intentionally to test exception handling.");

        return outputs;
    }
}
