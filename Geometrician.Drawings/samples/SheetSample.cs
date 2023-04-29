using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Geometrician.Core.Assets;
using Geometrician.Diagnostics.Samples;
using Geometrician.Flex;
using StudioLE.Workflows.Abstractions;

namespace Geometrician.Drawings.Samples;

[DisplayName(nameof(SheetSample))]
[Description(nameof(SheetSample))]
public class SheetSample : IActivity<SheetSample.Inputs, SheetSample.Outputs>
{
    public class Inputs
    {
        public ViewInputs View { get; set; } = new();
        public SheetInputs Sheet { get; set; } = new();
        public ArrangementInputs Arrangement { get; set; } = new();
    }


    public class ViewInputs
    {
        [Required]
        [Range(0, 50)]
        public int Scale { get; set; } = 10;

        [Required]
        [Range(0, .5)]
        public double Padding { get; set; } = .25;
    }

    public class SheetInputs
    {
        [Required]
        [Range(0, 2)]
        public double Width { get; set; } = .841;

        [Required]
        [Range(0, 2)]
        public double Height { get; set; } = .594;

        [Required]
        [Range(0, 2)]
        public double Title { get; set; } = .075;
    }

    public class ArrangementInputs
    {
        [Required]
        public Justification MainJustification { get; set; } = Justification.SpaceEvenly;

        [Required]
        public Justification CrossJustification { get; set; } = Justification.SpaceEvenly;

        [Required]
        public Alignment CrossAlignment { get; set; } = Alignment.End;
    }

    public class Outputs
    {
        public Model Model { get; set; } = new();

        public List<IAsset> Assets { get; } = new();
    }

    public Task<Outputs> Execute(Inputs inputs)
    {
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Left,
            ViewDirection.Right,
            ViewDirection.Front,
            ViewDirection.Back
        };

        Model model = Scenes.FromJson(Scenes.Name.Brickwork);

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(inputs.View.Padding, inputs.View.Padding, inputs.View.Padding)
            .Scale(1d / inputs.View.Scale)
            .ElementsInView(model.Elements.Values.ToArray());

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        SequenceBuilder sequenceBuilder = new();
        DefaultViewArrangement viewArrangement = new();

        viewArrangement
            .MainJustification(inputs.Arrangement.MainJustification)
            .CrossJustification(inputs.Arrangement.CrossJustification)
            .CrossAlignment(inputs.Arrangement.CrossAlignment);

        IBuilder<Sheet> builder = new SheetBuilder(sequenceBuilder, viewArrangement)
            .SheetSize(inputs.Sheet.Width, inputs.Sheet.Height)
            .VerticalTitleArea(inputs.Sheet.Title)
            .Views(views);
        Sheet sheet = builder.Build();

        // Prepare outputs
        Outputs outputs = new();
        outputs.Model.AddSubElements(sheet);
        outputs.Model.AddElements(sheet);

        return Task.FromResult(outputs);
    }
}
