using System.ComponentModel.DataAnnotations;

namespace Lineweights.Flex.Samples;

public static class WallStretcherBond
{
    public class Inputs
    {
        [Required]
        [Range(0, 50)]
        public double WallLength { get; set; } = 2;

        [Required]
        [Range(0, 1)]
        public double WallDepth { get; set; } = .300;

        [Required]
        [Range(0, 50)]
        public double WallHeight { get; set; } = 1;
    }

    public class Outputs
    {
        public Model Model { get; set; } = new();
    }

    public static Outputs Execute(Inputs inputs)
    {
        Line line = new(Vector3.Origin, Vector3.XAxis, inputs.WallLength);
        StandardWall wall = new(line, inputs.WallDepth, inputs.WallHeight);

        ISequenceBuilder sequenceA = new SequenceBuilder()
            .Repetition(true)
            .Overflow(true)
            .Body(Brick.Stretcher);
        ISequenceBuilder sequenceB = new SequenceBuilder()
            .Repetition(true)
            .Overflow(true)
            .Body(Brick.Stretcher)
            .Append(Brick.Half)
            .Prepend(Brick.Half);
        ISequenceBuilder crossSequence = new SequenceBuilder()
            .Repetition(true)
            .Overflow(true);

        // Configure the builder
        Flex2d builder = new Flex2d()
            .Orientation(Vector3.XAxis, Vector3.ZAxis, Vector3.YAxis)
            .MainJustification(Justification.Start)
            .CrossJustification(Justification.Start)
            .CrossAlignment(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .CrossSequence(crossSequence)
            .SetMainSequence(sequenceA, sequenceB)
            .SetContainer(wall);

        // Run the builder
        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = builder.Build();

        // Prepare outputs
        ElementInstance[] componentsFlattened = components.SelectMany(x => x).ToArray();
        Outputs outputs = new();
        outputs.Model.AddBounds(wall, MaterialByName("Gray"));
        outputs.Model.AddElements(componentsFlattened);
        outputs.Model.AddBounds(componentsFlattened);

        // Write sample file
#if false
// TODO: remove this
        Scenes.ToJson(Scenes.Name.Brickwork, componentsFlattened);
#endif

        return outputs;
    }
}
