using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Geometrician.Core.Assets;
using Geometrician.Diagnostics.Samples;
using Geometrician.Drawings;
using StudioLE.Workflows.Abstractions;

namespace Geometrician.Workflows.Samples;

[DisplayName(nameof(AssetTypes))]
[Description(nameof(AssetTypes))]
public class AssetTypes : IActivity<AssetTypes.Inputs, AssetTypes.Outputs>
{
    public sealed class Inputs
    {
        [Required]
        public Scenes.Name Scene { get; set; } = Scenes.Name.GeometricElements;

        [Required]
        public bool IncludeViews { get; set; } = true;

        [Required]
        [Compare(nameof(IncludeViews))]
        public bool IncludeSheet { get; set; } = true;

        [Required]
        public bool IncludeInternalAsset { get; set; } = true;

        [Required]
        public string InternalAssetContent { get; set; } = "This text is stored as a string in the InternalAsset.";

        [Required]
        public bool IncludeExternalAsset { get; set; } = true;

        [Required]
        public string ExternalAssetContent { get; set; } = "This text is stored in an external file referenced by the ExternalAsset.";

        [Required]
        public bool IncludeAssetsInModel { get; set; } = false;

        [Required]
        public bool ThrowAnException { get; set; } = false;
    }

    public sealed class Outputs
    {
        public Model Model { get; set; } = new();

        public ExternalAsset? ExternalAsset { get; set; }

        public InternalAsset? InternalAsset { get; set; }
    }

    public Task<Outputs> Execute(Inputs inputs)
    {
        Outputs outputs = new()
        {
            Model = Scenes.FromJson(inputs.Scene)
        };

        if (inputs.IncludeViews)
        {
            IReadOnlyCollection<View> views = SampleHelpers.CreateViews(outputs.Model, inputs.Scene);
            outputs.Model.AddElements(views);
        }

        if (inputs.IncludeViews && inputs.IncludeSheet)
        {
            Sheet sheet = SampleHelpers.CreateSheet(outputs.Model);
            outputs.Model.AddElement(sheet);
        }

        if (inputs.IncludeExternalAsset)
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, inputs.ExternalAssetContent);
            outputs.ExternalAsset = new()
            {
                Name = "External asset",
                Description = "This asset is stored as a property.",
                ContentType = "text/plain",
                Location = new(path)
            };
        }

        if (inputs.IncludeInternalAsset)
            outputs.InternalAsset = new()
            {
                Name = "Internal asset",
                Description = "This asset is stored as a property.",
                ContentType = "text/plain",
                Content = inputs.InternalAssetContent
            };

        if (inputs.IncludeAssetsInModel && outputs.ExternalAsset is not null)
        {
            outputs.ExternalAsset.Description = "This asset is stored in the model";
            outputs.Model.AddElement(outputs.ExternalAsset);
            outputs.ExternalAsset = null;
        }

        if (inputs.IncludeAssetsInModel && outputs.InternalAsset is not null)
        {
            outputs.InternalAsset.Description = "This asset is stored in the model";
            outputs.Model.AddElement(outputs.InternalAsset);
            outputs.InternalAsset = null;
        }

        if (inputs.ThrowAnException)
            throw new("This exception is thrown intentionally to test exception handling.");

        return Task.FromResult(outputs);
    }
}
