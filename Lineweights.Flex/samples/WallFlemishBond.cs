using System.ComponentModel.DataAnnotations;
using Lineweights.Flex.Sequences;

namespace Lineweights.Flex.Samples;

public static class WallFlemishBond
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

        // Configure the pattern
        var patternA = RepeatingSequence.WithoutOverflow(Brick.Stretcher.CreateInstance(), Brick.Half.CreateInstance())
            .AddConstraint(new IsOdd())
            .ConstraintMode(ConstraintMode.Or);
        var patternB = RepeatingSequence.WithoutOverflow(Brick.Half.CreateInstance(), Brick.Stretcher.CreateInstance())
            .AddConstraint(new IsOdd())
            .ConstraintMode(ConstraintMode.Or);

        // Configure the builder
        Flex2d builder = new Flex2d()
            .Container(wall)
            .Orientation(Vector3.XAxis, Vector3.ZAxis, Vector3.YAxis)
            .MainJustification(Justification.Center)
            .CrossJustification(Justification.Start)
            .CrossAlignment(Alignment.Center)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainPatterns(patternA, patternB)
            .CrossPattern(RepeatingSequence.WithOverflow());

        // Run the builder
        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = builder.Build();

        // Prepare outputs
        ElementInstance[] componentsFlattened = components.SelectMany(x => x).ToArray();
        Outputs outputs = new();
        outputs.Model.AddBounds(wall, MaterialByName("Gray"));
        outputs.Model.AddElements(componentsFlattened);
        outputs.Model.AddBounds(componentsFlattened);
        return outputs;
    }
}
