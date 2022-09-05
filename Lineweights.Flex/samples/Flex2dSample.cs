using System.ComponentModel.DataAnnotations;
using Lineweights.Flex.Sequences;
using StudioLE.Core.Exceptions;

namespace Lineweights.Flex.Samples;

public static class Flex2dSample
{
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
        [Required]
        public int MaxCount { get; set; } = 20;

        [Required]
        public SequenceType Type { get; set; } = SequenceType.StretcherSoldier;

        public enum SequenceType
        {
            StretcherSoldier,
            SoldierStretcher,
            StretcherHeader
        }
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

    public static Outputs Execute(
        ContainerInputs containerInputs,
        SequenceInputs firstSequenceInputs,
        SequenceInputs secondSequenceInputs,
        FlexInputs flexInputs,
        DisplayInputs displayInputs)
    {
        Brick container = new(containerInputs.Width, containerInputs.Length, containerInputs.Height, containerInputs.Spacing, "Container")
        {
            Material = MaterialByName("Gray")
        };

        SequenceBuilder firstSequence = GetSequenceByInputs(firstSequenceInputs);
        SequenceBuilder secondSequence = GetSequenceByInputs(secondSequenceInputs);

        Flex2d builder = new Flex2d()
            .Container(container)
            .MainJustification(flexInputs.MainJustification)
            .CrossJustification(flexInputs.CrossJustification)
            .CrossAlignment(flexInputs.CrossAlignment)
            .NormalAlignment(flexInputs.NormalAlignment)
            .NormalSettingOut(flexInputs.NormalSettingOut)
            .MainPatterns(firstSequence, secondSequence);

        Outputs outputs = new();

        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = builder.Build();

        if (displayInputs.ShowAssemblies)
            foreach (ElementInstance instance in builder.Assemblies)
            {
                outputs.Model.AddBounds(instance, MaterialByName("Aqua"));
                outputs.Model.AddBounds(instance.BaseDefinition, MaterialByName("Orange"));
            }

        if (displayInputs.ShowComponents)
        {
            outputs.Model.AddElements(components.SelectMany(x => x));
            outputs.Model.AddBounds(components.SelectMany(x => x));
        }

        return outputs;
    }

    private static SequenceBuilder GetSequenceByInputs(SequenceInputs inputs)
    {
        ElementInstance stretcher = Brick.Stretcher.CreateInstance();
        ElementInstance soldier = Brick.Soldier.CreateInstance();
        ElementInstance header = Brick.Header.CreateInstance();
        return inputs.Type switch
        {
            SequenceInputs.SequenceType.StretcherSoldier => RepeatingSequence.MaxCount(inputs.MaxCount, stretcher, soldier),
            SequenceInputs.SequenceType.SoldierStretcher => RepeatingSequence.MaxCount(inputs.MaxCount, soldier, stretcher),
            SequenceInputs.SequenceType.StretcherHeader => RepeatingSequence.MaxCount(inputs.MaxCount, stretcher, header),
            _ => throw new EnumSwitchException<SequenceInputs.SequenceType>($"Failed to {nameof(GetSequenceByInputs)}", inputs.Type)
        };
    }
}
