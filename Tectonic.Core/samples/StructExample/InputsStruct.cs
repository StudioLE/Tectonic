using System.ComponentModel.DataAnnotations;

namespace Tectonic.Core.Samples.StructExample;

public class InputsStruct
{
    [Required]
    public string StringValue { get; set; } = string.Empty;

    [Required]
    [Range(10, 20)]
    public int IntegerValue { get; set; }

    [Required]
    [Range(0, 1)]
    public double DoubleValue { get; set; }

    public ClassExample.ExampleEnum EnumValue { get; set; }

    public NestedStruct Nested { get; set; } = new();
}
