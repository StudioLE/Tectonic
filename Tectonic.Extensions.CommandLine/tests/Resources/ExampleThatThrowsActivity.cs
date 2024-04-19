namespace Tectonic.Extensions.CommandLine.Tests.Resources;

public class ExampleThatThrowsActivity : ActivityBase<ExampleClass, ExampleClass>
{
    public override Task<ExampleClass?> Execute(ExampleClass example)
    {
        throw new("This activity intentionally throws an exception.");
    }
}
