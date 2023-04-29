using StudioLE.Workflows.Samples.Resources;

namespace StudioLE.Workflows.Samples;

public static class StaticMethodActivityExample
{
    public sealed class Inputs : ExampleClass
    {
    }

    public sealed class Outputs
    {
        public bool IsValid { get; set; } = true;
    }

    public static Outputs Execute(Inputs inputs)
    {
        return new();
    }
}
