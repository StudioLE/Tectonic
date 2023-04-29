using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Cascade.Workflows.Abstractions;

namespace Geometrician.Flex.Samples;

[DisplayName(nameof(WallFlemishBond))]
[Description(nameof(WallFlemishBond))]
public class WallFlemishBond : IActivity<WallFlemishBond.Inputs, WallFlemishBond.Outputs>
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

    public Task<Outputs> Execute(Inputs inputs)
    {
        Line line = new(Vector3.Origin, Vector3.XAxis, inputs.WallLength);
        StandardWall wall = new(line, inputs.WallDepth, inputs.WallHeight);

        // Configure the sequence
        SequenceBuilder sequenceA = new SequenceBuilder()
            .Repetition(true)
            .OddCondition()
            .SetBody(Brick.Stretcher, Brick.Half);
        SequenceBuilder sequenceB = new SequenceBuilder()
            .Repetition(true)
            .OddCondition()
            .SetBody(Brick.Half, Brick.Stretcher);
        SequenceBuilder crossSequence = new SequenceBuilder()
            .Repetition(true)
            .Overflow(true);

        // Configure the builder
        Flex2d builder = new Flex2d()
            .Orientation(Vector3.XAxis, Vector3.ZAxis, Vector3.YAxis)
            .MainJustification(Justification.Center)
            .CrossJustification(Justification.Start)
            .CrossAlignment(Alignment.Center)
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
        return Task.FromResult(outputs);
    }
}
