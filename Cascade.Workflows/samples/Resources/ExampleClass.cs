using System.ComponentModel.DataAnnotations;

namespace Cascade.Workflows.Samples.Resources;

public class ExampleClass
{
    [Required]
    public string StringValue { get; set; } = string.Empty;

    [Required]
    [Range(10, 20)]
    public int IntegerValue { get; set; }

    [Required]
    [Range(0, 1)]
    public double DoubleValue { get; set; }

    public ExampleNestedClass Nested { get; set; } = new();
}
