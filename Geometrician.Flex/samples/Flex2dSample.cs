using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Cascade.Workflows;
using StudioLE.Core.Exceptions;

namespace Geometrician.Flex.Samples;

[DisplayName(nameof(Flex2dSample))]
[Description(nameof(Flex2dSample))]
public class Flex2dSample : IActivity<Flex2dSample.Inputs, Flex2dSample.Outputs>
{
    public class Inputs
    {
        public ContainerInputs Container { get; set; } = new();
        public SequenceInputs FirstSequence { get; set; } = new();
        public SequenceInputs SecondSequence { get; set; } = new();
        public FlexInputs Flex { get; set; } = new();
        public DisplayInputs Display { get; set; } = new();
    }

    public class ContainerInputs
    {
        [Required]
        public double Width { get; set; } = 4;

        [Required]
        public double Length { get; set; } = 1;

        [Required]
        public double Height { get; set; } = 1;

        [Required]
        public double Spacing { get; set; } = 0;
    }

    public class SequenceInputs
    {
        public enum SequenceType
        {
            StretcherSoldier,
            SoldierStretcher,
            StretcherHeader
        }

        [Required]
        public int MaxCount { get; set; } = 20;

        [Required]
        public SequenceType Type { get; set; } = SequenceType.StretcherSoldier;
    }

    public class FlexInputs
    {
        [Required]
        public Justification MainJustification { get; set; }

        [Required]
        public Justification CrossJustification { get; set; }

        [Required]
        public Alignment CrossAlignment { get; set; }

        [Required]
        public Alignment NormalAlignment { get; set; }

        [Required]
        public Alignment NormalSettingOut { get; set; }
    }

    public class DisplayInputs
    {
        [Required]
        public bool ShowAssemblies { get; set; } = false;

        [Required]
        public bool ShowComponents { get; set; } = true;
    }

    public class Outputs
    {
        public Model Model { get; set; } = new();
    }

    public Task<Outputs> Execute(Inputs inputs)
    {
        Brick container = new(inputs.Container.Width, inputs.Container.Length, inputs.Container.Height, inputs.Container.Spacing, "Container")
        {
            Material = MaterialByName("Gray")
        };

        SequenceBuilder firstSequence = GetSequenceByInputs(inputs.FirstSequence);
        SequenceBuilder secondSequence = GetSequenceByInputs(inputs.SecondSequence);

        Flex2d builder = new Flex2d()
            .MainJustification(inputs.Flex.MainJustification)
            .CrossJustification(inputs.Flex.CrossJustification)
            .CrossAlignment(inputs.Flex.CrossAlignment)
            .NormalAlignment(inputs.Flex.NormalAlignment)
            .NormalSettingOut(inputs.Flex.NormalSettingOut)
            .SetMainSequence(firstSequence, secondSequence)
            .SetContainer(container);

        Outputs outputs = new();

        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = builder.Build();

        if (inputs.Display.ShowAssemblies)
            foreach (ElementInstance instance in builder.Assemblies)
            {
                outputs.Model.AddBounds(instance, MaterialByName("Aqua"));
                outputs.Model.AddBounds(instance.BaseDefinition, MaterialByName("Orange"));
            }

        if (inputs.Display.ShowComponents)
        {
            outputs.Model.AddElements(components.SelectMany(x => x));
            outputs.Model.AddBounds(components.SelectMany(x => x));
        }

        return Task.FromResult(outputs);
    }

    private static SequenceBuilder GetSequenceByInputs(SequenceInputs inputs)
    {
        SequenceBuilder sequence = new SequenceBuilder()
            .Repetition(true)
            .MaxCountConstraint(inputs.MaxCount);
        return inputs.Type switch
        {
            SequenceInputs.SequenceType.StretcherSoldier => sequence.SetBody(Brick.Stretcher, Brick.Soldier),
            SequenceInputs.SequenceType.SoldierStretcher => sequence.SetBody(Brick.Soldier, Brick.Stretcher),
            SequenceInputs.SequenceType.StretcherHeader => sequence.SetBody(Brick.Stretcher, Brick.Header),
            _ => throw new EnumSwitchException<SequenceInputs.SequenceType>($"Failed to {nameof(GetSequenceByInputs)}", inputs.Type)
        };
    }
}
