using System.ComponentModel.DataAnnotations;
using Lineweights.Flex.Samples.Elements;
using Lineweights.Flex.Sequences;

namespace Lineweights.Flex.Samples;

public static class Seating2dAlternating
{
    public class Inputs
    {
        [Required]
        [Range(0, 50)]
        public double AuditoriumWidth { get; set; } = 3;

        [Required]
        [Range(0, 50)]
        public double AuditoriumLength { get; set; } = 5;

        [Required]
        [Range(0, 50)]
        public double AuditoriumHeight { get; set; } = 5;
    }

    public class Outputs
    {
        public Model Model { get; set; } = new();
    }

    public static Outputs Execute(Inputs inputs)
    {
        ElementInstance seat = new Seat("Seat").CreateInstance();
        Space auditorium = new(Polygon.Rectangle(inputs.AuditoriumWidth, inputs.AuditoriumLength), inputs.AuditoriumHeight);

        // Configure the pattern
        var patternA = RepeatingSequence.WithoutOverflow(seat);
        var patternB = RepeatingSequence
            .WithoutOverflow(seat)
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

        // Prepare the outputs
        Outputs outputs = new();
        outputs.Model.AddBounds(auditorium, MaterialByName("Gray"));
        outputs.Model.AddElements(components.SelectMany(x => x));
        return outputs;
    }
}
