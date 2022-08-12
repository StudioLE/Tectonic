using Lineweights.Flex.Samples.Elements;
using Lineweights.Flex.Sequences;
using Lineweights.Workflows.Results;

namespace Lineweights.Flex.Samples;

[SendToDashboardAfterTest]
internal sealed class SeatingSamples : ResultModel
{
    private ElementInstance Seat { get; } = new Seat("Seat").CreateInstance();

    [Test]
    public void Seating_1d_Linear()
    {
        Line line = new(Vector3.Origin, Vector3.XAxis, 10);

        // Configure the pattern
        var pattern = RepeatingSequence.WithoutOverflow(Seat);

        // Configure the builder
        Flex1d builder = new Flex1d()
            .Bounds(line, Vector3.YAxis)
            .MainJustification(Justification.Center)
            .CrossAlignment(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Pattern(pattern);

        // Run the builder
        IReadOnlyCollection<ElementInstance> components = builder.ToComponents();

        // Preview
        Model.AddElements(line, MaterialByName("Gray"));
        Model.AddElements(components);

        // TODO: Seats are unexpectedly hovering above ground. Possibly an origin issue?

        // Assert
        Verify.ElementsByBounds(components);
    }

    [Test]
    public void Seating_1d_Radial()
    {
        Arc arc = new(10, 0, 30);

        // Configure the pattern
        var pattern = RepeatingSequence.WithoutOverflow(Seat);

        // Configure the builder
        Flex1d builder = new Flex1d()
            .Bounds(arc)
            .MainJustification(Justification.Center)
            .CrossAlignment(Alignment.Start)
            .CrossSettingOut(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Pattern(pattern);

        // Run the builder
        IReadOnlyCollection<ElementInstance> components = builder.ToComponents();

        // Rotate the seats
        Transform rotation = new(Vector3.Origin, 180);
        var rotatedComponents = components.Select(instance => instance
            .BaseDefinition
            .CreateInstance(
                rotation.Concatenated(instance.Transform),
                instance.Name));

        // TODO: Seats are unexpectedly hovering above ground. Possibly an origin issue?
        // TODO: Radial rotation is inset whereas outset would be better. May need to flip the axis.

        // Preview
        Model.AddElements(arc, MaterialByName("Gray"));
        Model.AddElements(rotatedComponents);

        // Assert
        Verify.ElementsByBounds(rotatedComponents);
    }

    [Test]
    public void Seating_2d_Alternating()
    {
        // Arrange
        const double width = 3;
        const double length = 5;
        const double height = 5;
        Space auditorium = new(Polygon.Rectangle(width, length), height);

        // Configure the pattern
        var patternA = RepeatingSequence.WithoutOverflow(Seat);
        var patternB = RepeatingSequence
            .WithoutOverflow(Seat)
            .AdjustTakeCount(-1);

        // Configure the builder
        Flex2d builder = new Flex2d()
            .Container(auditorium)
            .MainJustification(Justification.Center)
            .CrossJustification(Justification.SpaceBetween)
            .CrossAlignment(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainPatterns(patternA, patternB)
            .CrossPattern(RepeatingSequence.WithoutOverflow());

        // TODO: Chairs are being crated upside down...

        // Run the builder
        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = builder.ToComponents();

        // Preview
        Model.AddBounds(auditorium, MaterialByName("Gray"));
        Model.AddElements(components.SelectMany(x => x));

        // Assert
        Verify.ElementsByBounds(components.SelectMany(x => x));
    }
}
