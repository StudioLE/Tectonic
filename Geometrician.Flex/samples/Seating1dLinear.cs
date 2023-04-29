using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Geometrician.Flex.Samples.Elements;
using StudioLE.Workflows.Abstractions;

namespace Geometrician.Flex.Samples;

[DisplayName(nameof(Seating1dLinear))]
[Description(nameof(Seating1dLinear))]
public class Seating1dLinear : IActivity<Seating1dLinear.Inputs,Seating1dLinear.Outputs>
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

    public Task<Outputs> Execute(Inputs inputs)
    {
        Line line = new(Vector3.Origin, Vector3.XAxis, inputs.LineLength);
        ElementInstance seat = new Seat("Seat").CreateInstance();

        // Configure the pattern
        SequenceBuilder sequence = new SequenceBuilder()
            .Repetition(true)
            .SetBody(seat);

        // Configure the builder
        Flex1d builder = new Flex1d()
            .Bounds(line, Vector3.YAxis)
            .MainJustification(Justification.Center)
            .CrossAlignment(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Sequence(sequence);

        // Run the builder
        IReadOnlyCollection<ElementInstance> components = builder.Build();

        // Prepare the outputs
        Outputs outputs = new();
        outputs.Model.AddElements(line, MaterialByName("Gray"));
        outputs.Model.AddElements(components);

        // TODO: Seats are unexpectedly hovering above ground. Possibly an origin issue?
        return Task.FromResult(outputs);
    }
}
