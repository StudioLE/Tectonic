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

        // Configure the sequence
        ISequenceBuilder oddSequence = new SequenceBuilder()
            .Repetition(true)
            .OddCondition()
            .Body(seat);
        ISequenceBuilder evenSequence = new SequenceBuilder()
            .Repetition(true)
            .EvenCondition()
            .Body(seat);
        ISequenceBuilder crossSequence = new SequenceBuilder()
            .Repetition(true);

        // Configure the builder
        Flex2d builder = new Flex2d()
            .Container(auditorium)
            .MainJustification(Justification.Center)
            .CrossJustification(Justification.SpaceBetween)
            .CrossAlignment(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .MainSequence(oddSequence, evenSequence)
            .CrossSequence(crossSequence);

        // Run the builder
        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = builder.Build();

        // Prepare the outputs
        Outputs outputs = new();
        outputs.Model.AddBounds(auditorium, MaterialByName("Gray"));
        outputs.Model.AddElements(components.SelectMany(x => x));
        return outputs;
    }
}
