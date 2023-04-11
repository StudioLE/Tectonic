using System.ComponentModel.DataAnnotations;
using Geometrician.Core.Assets;
using Geometrician.Diagnostics.Samples;
using Geometrician.Flex;

namespace Geometrician.Drawings.Samples;

public static class SheetSample
{

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

    public static Outputs Execute(ViewInputs viewInputs, SheetInputs sheetInputs, ArrangementInputs arrangementInputs)
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
            .ScopePadding(viewInputs.Padding, viewInputs.Padding, viewInputs.Padding)
            .Scale(1d / viewInputs.Scale)
            .ElementsInView(model.Elements.Values.ToArray());

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        SequenceBuilder sequenceBuilder = new();
        DefaultViewArrangement viewArrangement = new();

        viewArrangement
            .MainJustification(arrangementInputs.MainJustification)
            .CrossJustification(arrangementInputs.CrossJustification)
            .CrossAlignment(arrangementInputs.CrossAlignment);

        IBuilder<Sheet> builder = new SheetBuilder(sequenceBuilder, viewArrangement)
            .SheetSize(sheetInputs.Width, sheetInputs.Height)
            .VerticalTitleArea(sheetInputs.Title)
            .Views(views);
        Sheet sheet = builder.Build();

        // Prepare outputs
        Outputs outputs = new();
        outputs.Model.AddSubElements(sheet);
        outputs.Model.AddElements(sheet);

        return outputs;
    }
}
