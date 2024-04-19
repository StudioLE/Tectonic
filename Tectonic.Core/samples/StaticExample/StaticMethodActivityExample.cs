using Tectonic.Core.Samples.ClassExample;

namespace Tectonic.Core.Samples.StaticExample;

public static class StaticMethodActivityExample
{
    public sealed class Inputs : InputsClass
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
