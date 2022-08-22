using Lineweights.Flex.Samples.Elements;
using Lineweights.Flex.Sequences;

namespace Lineweights.Flex.Samples;

public static class Seating1dRadial
{
    public class Outputs
    {
        public Model Model { get; set; } = new();
    }

    public static Outputs Execute()
    {
        Arc arc = new(10, 0, 30);
        ElementInstance seat = new Seat("Seat").CreateInstance();

        // Configure the pattern
        var pattern = RepeatingSequence.WithoutOverflow(seat);

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


        // Prepare the outputs
        Outputs outputs = new();
        outputs.Model.AddElements(arc, MaterialByName("Gray"));
        outputs.Model.AddElements(rotatedComponents);
        return outputs;
    }
}
