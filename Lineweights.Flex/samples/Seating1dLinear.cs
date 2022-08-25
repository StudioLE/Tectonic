using System.ComponentModel.DataAnnotations;
using Lineweights.Flex.Samples.Elements;
using Lineweights.Flex.Sequences;

namespace Lineweights.Flex.Samples;

public static class Seating1dLinear
{
    public class Inputs
    {
        [Required]
        [Range(0, 50)]
        public int LineLength { get; set; } = 10;
    }

    public class Outputs
    {
        public Model Model { get; set; } = new();
    }

    public static Outputs Execute(Inputs inputs)
    {
        Line line = new(Vector3.Origin, Vector3.XAxis, inputs.LineLength);
        ElementInstance seat = new Seat("Seat").CreateInstance();

        // Configure the pattern
        var pattern = RepeatingSequence.WithoutOverflow(seat);

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

        // Prepare the outputs
        Outputs outputs = new();
        outputs.Model.AddElements(line, MaterialByName("Gray"));
        outputs.Model.AddElements(components);

        // TODO: Seats are unexpectedly hovering above ground. Possibly an origin issue?
        return outputs;
    }
}
