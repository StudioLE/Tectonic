using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Geometrician.Flex.Samples.Elements;
using StudioLE.Workflows.Abstractions;

namespace Geometrician.Flex.Samples;

[DisplayName(nameof(Seating1dRadial))]
[Description(nameof(Seating1dRadial))]
public class Seating1dRadial : IActivity<Seating1dRadial.Inputs, Seating1dRadial.Outputs>
{

    public class Inputs
    {
        [Required]
        [Range(0, 50)]
        public int ArcRadius { get; set; } = 10;

        [Required]
        [Range(-180, 360)]
        public int ArcStartAngle { get; set; } = 0;

        [Required]
        [Range(-180, 360)]
        public int ArcEndAngle { get; set; } = 30;

        [Required]
        [Range(-180, 360)]
        public int Rotation { get; set; } = 180;
    }

    public class Outputs
    {
        public Model Model { get; set; } = new();
    }

    public Task<Outputs> Execute(Inputs inputs)
    {
        Arc arc = new(inputs.ArcRadius, inputs.ArcStartAngle, inputs.ArcEndAngle);
        ElementInstance seat = new Seat("Seat").CreateInstance();

        // Configure the pattern
        SequenceBuilder sequence = new SequenceBuilder()
            .Repetition(true)
            .SetBody(seat);

        // Configure the builder
        Flex1d builder = new Flex1d()
            .Bounds(arc)
            .MainJustification(Justification.Center)
            .CrossAlignment(Alignment.Start)
            .CrossSettingOut(Alignment.Start)
            .NormalAlignment(Alignment.Start)
            .NormalSettingOut(Alignment.Start)
            .Sequence(sequence);

        // Run the builder
        IReadOnlyCollection<ElementInstance> components = builder.Build();

        // Rotate the seats
        Transform rotation = new(Vector3.Origin, inputs.Rotation);
        IEnumerable<ElementInstance> rotatedComponents = components.Select(instance => instance
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
        return Task.FromResult(outputs);
    }
}
