namespace Tectonic.Core.Samples.StructExample;

public class OutputsStruct
{
    public ClassExample.ExampleEnum EnumValue { get; set; }

    public NestedStruct Nested { get; set; } = new();
}
