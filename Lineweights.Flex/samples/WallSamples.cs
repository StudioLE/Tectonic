using Lineweights.Flex.Sequences;
using Lineweights.Workflows.Results;

namespace Lineweights.Flex.Samples;

[SendToDashboardAfterTest]
internal sealed class WallSamples : ResultModel
{
    private StandardWall Wall { get; }

    public WallSamples()
    {
        const double length = 2;
        const double depth = .3;
        const double height = 1;
        Line line = new(Vector3.Origin, Vector3.XAxis, length);
        Wall = new(line, depth, height);
    }

    [Test]
    public void Wall_StretcherBond()
    {
        // Configure the pattern
        var patternA = RepeatingSequence.WithOverflow(Brick.Stretcher.CreateInstance());
        var patternB = RepeatingSequence.WithOverflow(Brick.Stretcher.CreateInstance())
            .PrependedItems(Brick.Half.CreateInstance())
            .AppendedItems(Brick.Half.CreateInstance());

        // Configure the builder
        Flex2d builder = new Flex2d()
            .Container(Wall)
            .Orientation(Vector3.XAxis, Vector3.ZAxis, Vector3.YAxis)
            .MainJustification(Justification.Start)
            .CrossJustification(Justification.Start)
            .CrossAlignment(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainPatterns(patternA, patternB)
            .CrossPattern(RepeatingSequence.WithOverflow());

        // Run the builder
        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = builder.ToComponents();

        // Preview
        ElementInstance[] componentsFlattened = components.SelectMany(x => x).ToArray();
        Model.AddBounds(Wall, MaterialByName("Gray"));
        Model.AddElements(componentsFlattened);
        Model.AddBounds(componentsFlattened);

        // Write sample file
#if false
        Scenes.ToJson(Scenes.Name.Brickwork, componentsFlattened);
#endif

        // Assert
        Verify.ElementsByBounds(componentsFlattened);
    }

    [Test]
    public void Wall_FlemishBond()
    {
        // Configure the pattern
        var patternA = RepeatingSequence.WithoutOverflow(Brick.Stretcher.CreateInstance(), Brick.Half.CreateInstance())
            .AddConstraint(new IsOdd())
            .ConstraintMode(ConstraintMode.Or);
        var patternB = RepeatingSequence.WithoutOverflow(Brick.Half.CreateInstance(), Brick.Stretcher.CreateInstance())
            .AddConstraint(new IsOdd())
            .ConstraintMode(ConstraintMode.Or);

        // Configure the builder
        Flex2d builder = new Flex2d()
            .Container(Wall)
            .Orientation(Vector3.XAxis, Vector3.ZAxis, Vector3.YAxis)
            .MainJustification(Justification.Center)
            .CrossJustification(Justification.Start)
            .CrossAlignment(Alignment.Center)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainPatterns(patternA, patternB)
            .CrossPattern(RepeatingSequence.WithOverflow());

        // Run the builder
        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = builder.ToComponents();

        // Preview
        ElementInstance[] componentsFlattened = components.SelectMany(x => x).ToArray();
        Model.AddBounds(Wall, MaterialByName("Gray"));
        Model.AddElements(componentsFlattened);
        Model.AddBounds(componentsFlattened);

        // Assert
        Verify.ElementsByBounds(components.SelectMany(x => x));
    }
}
