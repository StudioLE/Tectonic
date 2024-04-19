namespace Tectonic.Core.Samples.ClassExample;

public class OutputsClass
{
    public ExampleEnum EnumValue { get; set; }

    public NestedClass Nested { get; set; } = new();
}
