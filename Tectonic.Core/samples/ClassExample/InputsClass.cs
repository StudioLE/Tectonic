using System.ComponentModel.DataAnnotations;

namespace Tectonic.Core.Samples.ClassExample;

public class InputsClass
{
    [Required]
    public string StringValue { get; set; } = string.Empty;

    [Required]
    [Range(10, 20)]
    public int IntegerValue { get; set; }

    [Required]
    [Range(0, 1)]
    public double DoubleValue { get; set; }

    public ExampleEnum EnumValue { get; set; }

    public NestedClass Nested { get; set; } = new();
}
